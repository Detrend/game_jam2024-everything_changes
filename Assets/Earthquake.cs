using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
  [SerializeField]
  float Duration = 10.0f;

  float m_TimeRemaining = 10.0f;

  void Start()
  {
    m_TimeRemaining = Duration;
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

    Game.I.ScreenShaker.ShakeScreen(1.0f, 2.0f);
  }

  void Despawn()
  {
    GameObject.Destroy(gameObject);
  }
}
