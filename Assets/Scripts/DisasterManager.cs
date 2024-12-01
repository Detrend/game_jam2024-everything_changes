using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public enum DisasterType
{
  Zombies,
  Trump,
  Rain,
  Tornado,
  Earthquake,
  Floods,
  Ufo,
}

public class IncomingDisaster
{
  public DisasterType disaster;
  public float        remaining;
  public int          id;

  public IncomingDisaster(DisasterType disaster, float remaining, int id)
  {
    this.disaster  = disaster;
    this.remaining = remaining;
    this.id        = id;
  }
}

[Serializable]
public class DisasterSettings
{
  public DisasterType disaster;

  [Range(0.0f, 0.1f)] public float probability;
  public float intervalStart = 0.0f;

  // cooldown after start
  public float selfCooldown;
  public float otherCooldown;
}

public class DisasterManager : MonoBehaviour
{
  // ===          SERIALIZABLE          === //
  [SerializeField]
  public int RandomSeedOverride = -1;

  [SerializeField]
  public float DebugStartFrom = 0.0f;

  [SerializeField]
  public UnityEvent<DisasterType, int> OnDisasterStarted = new UnityEvent<DisasterType, int>();

  [SerializeField]
  public UnityEvent<DisasterType, int> OnDisasterScheduled = new UnityEvent<DisasterType, int>();

  [SerializeField]
  public List<DisasterSettings> Probabilities = new List<DisasterSettings>();

  // How long ahead do we know of a disaster
  [SerializeField]
  public float DisasterSpawnAheadTime = 20.0f;

  // ===              PRIVATE             === //

  Dictionary<DisasterType, DisasterSettings> m_Settings = new Dictionary<DisasterType, DisasterSettings>();
  Dictionary<DisasterType, float> m_Cooldowns           = new Dictionary<DisasterType, float>();

  bool          m_Debug = false;
  System.Random m_Randomizer;
  float         m_TimeSinceLevelStart = 0.0f;
  float         m_GlobalCooldown = 0.0f;
  int           m_DisasterCount = 0;

  void Awake()
  {
    if (RandomSeedOverride >= 0)
    {
      Debug.LogWarning(
        $"<color=yellow>[Disaster Manager] Warning:</color> Random seed is overriden!!!");
      m_Randomizer = new System.Random(RandomSeedOverride);
    }
    else
    {
      m_Randomizer = new System.Random();
    }
  }

  void Start()
  {
    if (Debug.isDebugBuild)
    {
      m_TimeSinceLevelStart = DebugStartFrom;
    }

    // initialize disaster map
    foreach (var cfg in Probabilities)
    {
      if (m_Settings.ContainsKey(cfg.disaster))
      {
        Debug.LogWarning(
          $"<color=yellow>[Disaster Manager] Warning:</color> Probability key \"{cfg.disaster}\" is duplicate");
      }
      m_Settings.Add(cfg.disaster, cfg);
    }

    // iterate all and fill the uninitialized with default values
    foreach (DisasterType key in Enum.GetValues(typeof(DisasterType)))
    {
      m_Cooldowns.Add(key, 0.0f);

      if (!m_Settings.ContainsKey(key))
      {
        m_Settings.Add(key, new DisasterSettings());
        Debug.LogWarning(
          $"<color=yellow>[Disaster Manager] Warning:</color> Probability key \"{key}\" not intialized");
      }
    }
  }

  void Update()
  {
    UpdateDebug();

    m_TimeSinceLevelStart += Time.deltaTime;

    // Update self cooldowns
    foreach (DisasterType type in Enum.GetValues(typeof(DisasterType)))
    {
      m_Cooldowns[type] = Mathf.Max(0.0f, m_Cooldowns[type] - Time.deltaTime);
    }

    // Update the global cooldown
    m_GlobalCooldown = Mathf.Max(0.0f, m_GlobalCooldown - Time.deltaTime);

    // Update the disaster probability
    m_TimeAccum += Time.deltaTime;
    while (m_TimeAccum > m_FixedUpdateTime)
    {
      DoFixedUpdate();
      m_TimeAccum -= m_FixedUpdateTime;
    }

    // Update incoming disasters and fire them potentially
    foreach (IncomingDisaster incoming in m_DisastersToCome)
    {
      incoming.remaining -= Time.deltaTime;
      if (incoming.remaining <= 0.0f)
      {
        OnDisasterStartedInternal(incoming.disaster, incoming.id);
      }
    }

    // Remove the expired ones
    m_DisastersToCome.RemoveAll(item => item.remaining <= 0.0f);
  }

  float GetDisasterProbability(DisasterType type)
  {
    if (m_TimeSinceLevelStart < m_Settings[type].intervalStart)
    {
      return 0.0f;
    }

    if (m_GlobalCooldown > 0.0f)
    {
      return 0.0f;
    }

    if (m_Cooldowns[type] > 0.0f)
    {
      return 0.0f;
    }

    // 6 min = 360sec
    float maxPlayTime = 360.0f;
    float coeff = Mathf.Clamp(m_TimeSinceLevelStart / maxPlayTime, 0.0f, 1.0f) + 1.0f;

    return m_Settings[type].probability * coeff;
  }

  void OnDisasterScheduledInternal(DisasterType type, int id)
  {
    float cooldown = m_Settings[type].otherCooldown;
    m_GlobalCooldown = Mathf.Max(cooldown, m_GlobalCooldown);

    m_Cooldowns[type] = m_Settings[type].selfCooldown;

    OnDisasterScheduled.Invoke(type, id);
    Debug.Log($"[Disaster Manager] Info: disaster of type \"{type}\" was scheduled.");
  }

  void OnDisasterStartedInternal(DisasterType type, int id)
  {
    OnDisasterStarted.Invoke(type, id);
    Debug.Log($"[Disaster Manager] Info: disaster of type \"{type}\" started.");
  }

  void UpdateDebug()
  {
    if (!Debug.isDebugBuild)
    {
      return;
    }

    if (Input.GetKeyDown(KeyCode.F1))
    {
      m_Debug = !m_Debug;
    }

    for (var startCode = KeyCode.F2; startCode <= KeyCode.F8; ++startCode)
    {
      if (Input.GetKeyDown(startCode))
      {
        OnDisasterStartedInternal(DisasterType.Zombies + (startCode - KeyCode.F2), -1);
      }
    }
  }

  void OnGUI()
  {
    if (!m_Debug)
    {
      return;
    }

    float y = 30.0f;

    GUI.Label(new Rect(30.0f, y, 300.0f, 30.0f), $"[Time]: {m_TimeSinceLevelStart}");
    y += 30;
    GUI.Label(new Rect(30.0f, y, 300.0f, 30.0f), $"[Global cooldown]: {m_GlobalCooldown}");
    y += 30;
    GUI.Label(new Rect(30.0f, y, 300.0f, 30.0f), "[TYPE] : [PROB] : [ENABLED SINCE] : [COOLDOWN]");

    // draw probabilities
    foreach (DisasterType type in Enum.GetValues(typeof(DisasterType)))
    {
      y += 30;
      float prob = GetDisasterProbability(type);
      GUI.Label(new Rect(30.0f, y, 300.0f, 30.0f), $"[{type}] : {prob} : {m_Settings[type].intervalStart} : {m_Cooldowns[type]}");
    }

    y += 30;
    GUI.Label(new Rect(30.0f, y, 300.0f, 30.0f), "[TYPE] : [COUNTDOWN]");

    // draw incoming
    foreach (var incoming in m_DisastersToCome)
    {
      y += 30;
      float  countdown = incoming.remaining;
      string type      = incoming.disaster.ToString();
      GUI.Label(new Rect(30.0f, y, 300.0f, 30.0f), $"[{type}] : {countdown}");
    }
  }

  // Update the disasters here
  void DoFixedUpdate()
  {
    var disasterProbs = new List<Tuple<DisasterType, float>>();

    foreach (DisasterType type in Enum.GetValues(typeof(DisasterType)))
    {
      float probability = GetDisasterProbability(type);
      if (probability > 0.0f)
      {
        disasterProbs.Add(Tuple.Create(type, probability));
      }
    }

    // now generate a random number
    float rnd = (float)m_Randomizer.NextDouble();

    float startFrom = 0.0f;
    foreach (var prob in disasterProbs)
    {
      float endAt = startFrom + prob.Item2;

      if (rnd >= startFrom && rnd < endAt)
      {
        // spawn this event
        var id = ++m_DisasterCount;
        m_DisastersToCome.Add(new IncomingDisaster(prob.Item1, DisasterSpawnAheadTime, id));
        OnDisasterScheduledInternal(prob.Item1, id);
        break;
      }

      startFrom += prob.Item2;
    }
  }

  List<IncomingDisaster> m_DisastersToCome = new List<IncomingDisaster>();

  public List<IncomingDisaster> GetIncoming() => m_DisastersToCome;

  // Accumulate the update time
  float m_TimeAccum       = 0.0f;
  float m_FixedUpdateTime = 1.0f; // in seconds
  // TODO: queued disasters

  // How do the disasters work?
  // Every type of disaster should have some probablility which determines how often it happens.
  // Some disaster types should appear since later parts of the game.
  // We probably do not want more disasters at the same time. If so, then there might be some un-compatible disasters.
  // Disasters should probably not be grouped with each other.
  // Disasters should have some cooldown.
}
