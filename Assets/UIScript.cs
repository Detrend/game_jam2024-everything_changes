using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{
  [Serializable]
  public class TypeIconPair
  {
    [SerializeField] public DisasterType type;
    [SerializeField] public GameObject   icon;
  }

  [SerializeField]
  public List<TypeIconPair> Icons;

  Dictionary<int, GameObject>          m_Icons = new Dictionary<int, GameObject>();
  Dictionary<DisasterType, GameObject> m_IconStorage = new Dictionary<DisasterType, GameObject>();

  Vector3 m_Left  = Vector3.zero;
  Vector3 m_Right = Vector3.zero;

  public void OnDisasterScheduled(DisasterType type, int id)
  {
    m_Icons.Add(id, Instantiate(m_IconStorage[type], transform));
  }

  public void OnDisasterStarted(DisasterType type, int id)
  {
    m_Icons.Remove(id);
  }

  void Start()
  {
    m_Left  = transform.Find("Left").transform.position;
    m_Right = transform.Find("Right").transform.position;

    foreach (var pair in Icons)
    {
      m_IconStorage.Add(pair.type, pair.icon);
    }

    foreach (DisasterType type in Enum.GetValues(typeof(DisasterType)))
    {
      if (!m_IconStorage.ContainsKey(type))
      {
        Debug.LogError($"[UI script: no icon for type \"{type}\"!!!]");
      }
    }
  }

  void Update()
  {
    var currentEvents = Game.I.DisasterManager.GetIncoming();
    foreach (var evnt in currentEvents)
    {
      var id = evnt.id;
      if (m_Icons.ContainsKey(id))
      {
        float prog = evnt.remaining / Game.I.DisasterManager.DisasterSpawnAheadTime;
        m_Icons[id].transform.position = Vector3.Lerp(m_Left, m_Right, prog);
      }
    }
  }
}
