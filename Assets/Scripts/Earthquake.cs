using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
  [SerializeField]
  float Duration = 10.0f;

  float m_TimeRemaining = 10.0f;

    [SerializeField]
    float earthquakeStrength = 1.0f;

    ScreenShake _earthquakeScreenShake;

  void Start()
  {
        m_TimeRemaining = Duration;
        _earthquakeScreenShake = new(0.4f)
        {
            _shakeVectors = new()
            {
                (new Vector2(1f, 0.1f), 70f),
                (new Vector2(0.2f, 1f), 9f)
            }
        };
        Game.I.ScreenShaker.AddShake(_earthquakeScreenShake);
    }

  void Update()
  {
    m_TimeRemaining -= Time.deltaTime;

    if (m_TimeRemaining < 0.0f)
    {
      Despawn();
    }
    else
    {
      UpdateEarthquake();
    }
  }

  void UpdateEarthquake()
  {
    // drain HP from all blocks
    var blocks = Game.I.HouseGrid.AllBlocks;
    foreach (var block in blocks)
    {
      if (block)
      {
        // drain HP
      }
    }

    _earthquakeScreenShake._amount = earthquakeStrength;
  }

  void Despawn()
  {
    Destroy(gameObject);
  }
}
