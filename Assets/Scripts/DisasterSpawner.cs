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
  GameObject RainObject;

  [SerializeField]
  GameObject UfoObject;

  [SerializeField]
  GameObject EarthquakeObject;

  [SerializeField]
  GameObject TornadoObject;

  [SerializeField]
  GameObject FloodObject;

  [SerializeField]
  int SeedOverride = -1;

  // PRIVATE //
  System.Random m_Randomizer;

  public void OnDisasterStarted(DisasterType type, int id)
  {
    switch (type)
    {
      case DisasterType.Zombies:
      {
        StartCoroutine(SpawnZombieWaves(3, 4));
        break;
      }

      case DisasterType.Trump:
      {
        StartCoroutine(SpawnZombieWaves(5, 7));
        break;
      }

      case DisasterType.Rain:
      {
        StartRain();
        break;
      }

      case DisasterType.Tornado:
      {
        StartTornado();
        break;
      }

      case DisasterType.Earthquake:
      {
        StartEarthquake();
        break;
      }

      case DisasterType.Floods:
      {
        StartFloods();
        break;
      }

      case DisasterType.Ufo:
      {
        StartUfo();
        break;
      }
    }
  }

  public void OnDisasterScheduled(DisasterType type, int id)
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

  void StartRain()
  {
    Instantiate(RainObject, transform.position, transform.rotation);
  }

  void StartTornado()
  {
    Instantiate(TornadoObject, transform.position, transform.rotation);
  }

  void StartFloods()
  {
    Instantiate(FloodObject, transform.position, transform.rotation);
  }

  void StartEarthquake()
  {
    Instantiate(EarthquakeObject, transform.position, transform.rotation);
  }

  void SpawnZombie()
  {
    Instantiate(ZombieObject, transform.position, transform.rotation);
  }

  void StartUfo()
  {
    Instantiate(UfoObject, transform.position, transform.rotation);
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
