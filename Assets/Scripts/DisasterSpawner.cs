using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterSpawner : MonoBehaviour
{
  // SERIALIZABLE //

  [SerializeField]
  GameObject ZombieObject;

  [SerializeField]
  GameObject TrumpObject;

  [SerializeField]
  GameObject StormObject;

  [SerializeField]
  int SeedOverride = -1;

  // PRIVATE //
  System.Random m_Randomizer;

  public void OnDisasterStarted(DisasterType type)
  {
    switch (type)
    {
      case DisasterType.Zombies:
      {
        StartCoroutine(SpawnZombieWaves(3, 4));
        break;
      }

      case DisasterType.Storm:
      {
        StartStorm();
        break;
      }

      case DisasterType.Trump:
      {
        StartCoroutine(SpawnZombieWaves(2, 5));
        break;
      }
    }
  }

  public void OnDisasterScheduled(DisasterType type)
  {
    
  }

  IEnumerator SpawnZombieWaves(int waveCount, int waveSize)
  {
    for (int wave = 0; wave < waveCount; ++wave)
    {
      for (int i = 0; i < waveSize; ++i)
      {
        SpawnZombie();
        yield return new WaitForSeconds((float)m_Randomizer.NextDouble() * 1.0f);
      }

      yield return new WaitForSeconds((float)m_Randomizer.NextDouble() * 1.5f + 0.5f);
    }

    yield return null;
  }

  void StartStorm()
  {
    Instantiate(StormObject, transform);
  }

  void SpawnZombie()
  {
    Instantiate(ZombieObject, transform);
  }

  void Awake()
  {
    if (SeedOverride < 0)
    {
      m_Randomizer = new System.Random();
    }
    else
    {
      m_Randomizer = new System.Random(SeedOverride);
      Debug.LogWarning("<color=yellow> [Disaster Spawner] Warning: </color> Seed is not randomized!!!");
    }
  }

  void Start()
  {
    
  }

  void Update()
  {
    
  }
}
