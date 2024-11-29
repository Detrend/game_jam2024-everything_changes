using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public enum DisasterType
{
  Zombies,
  Storm,

  // TODO
  Trump,
  Tornado,
  Ufo,
}

public class IncomingDisaster
{
  public DisasterType disaster;
  public float        remaining;

  public IncomingDisaster(DisasterType disaster, float remaining)
  {
    this.disaster  = disaster;
    this.remaining = remaining;
  }
}

[Serializable]
public class DisasterSettings
{
  public DisasterType disaster;

  [Range(0.0f, 0.1f)] public float probability;

  // cooldown after start
  public float cooldown;
  public float enabledSince  = 0.0f;
  public float disabledSince = 0.0f;
}

public class DisasterManager : MonoBehaviour
{
  [SerializeField]
  public int RandomSeedOverride = -1;

  [SerializeField]
  public UnityEvent<DisasterType> OnDisasterStarted = new UnityEvent<DisasterType>();

  [SerializeField]
  public UnityEvent<DisasterType> OnDisasterScheduled = new UnityEvent<DisasterType>();

  [SerializeField]
  public List<DisasterSettings> Probabilities = new List<DisasterSettings>();

  // How long ahead do we know of a disaster
  [SerializeField]
  public float DisasterSpawnAheadTime = 20.0f;

  Dictionary<DisasterType, DisasterSettings> m_Settings = new Dictionary<DisasterType, DisasterSettings>();

  void Start()
  {
    if (RandomSeedOverride >= 0)
    {
      Debug.LogWarning(
        $"<color=yellow>[Disaster Manager] Warning:</color> Random seed is overriden!!!");
      UnityEngine.Random.InitState(RandomSeedOverride);
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
        OnDisasterStartedInternal(incoming.disaster);
      }
    }

    // Remove the expired ones
    m_DisastersToCome.RemoveAll(item => item.remaining <= 0.0f);
  }

  float GetDisasterProbability(DisasterType type)
  {
    return m_Settings[type].probability;
  }

  void OnDisasterScheduledInternal(DisasterType type)
  {
    OnDisasterScheduled.Invoke(type);
    Debug.Log($"[Disaster Manager] Info: disaster of type \"{type}\" was scheduled.");
  }

  void OnDisasterStartedInternal(DisasterType type)
  {
    OnDisasterStarted.Invoke(type);
    Debug.Log($"[Disaster Manager] Info: disaster of type \"{type}\" started.");
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
    float rnd = UnityEngine.Random.Range(0.0f, 1.0f);

    float startFrom = 0.0f;
    foreach (var prob in disasterProbs)
    {
      float endAt = startFrom + prob.Item2;

      if (rnd >= startFrom && rnd < endAt)
      {
        // spawn this event
        m_DisastersToCome.Add(new IncomingDisaster(prob.Item1, DisasterSpawnAheadTime));
        OnDisasterScheduledInternal(prob.Item1);
        break;
      }

      startFrom += prob.Item2;
    }
  }

  List<IncomingDisaster> m_DisastersToCome = new List<IncomingDisaster>();

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
