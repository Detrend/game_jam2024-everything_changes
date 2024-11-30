using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : MonoBehaviour
{
  [SerializeField]
  public int Height = 1;

  [SerializeField]
  public float Duration = 15.0f;

  float m_Duration = 0.0f;
  GameObject m_Water = null;

  void Start()
  {
    m_Duration = Duration;
    m_Water = transform.Find("Water")?.gameObject;

    StartCoroutine(StartingCoroutine());
  }

  IEnumerator StartingCoroutine()
  {
    for (int i = 0; i < 100; ++i)
    {
      m_Water.transform.position += new Vector3(0.0f, 1.0f, 0.0f) * 0.01f;
      yield return new WaitForSeconds(0.1f);
    }

    yield return null;
  }

  IEnumerator EndCoroutine()
  {
    for (int i = 0; i < 100; ++i)
    {
      m_Water.transform.position += new Vector3(0.0f, -1.0f, 0.0f) * 0.01f;
      yield return new WaitForSeconds(0.1f);
    }

    GameObject.Destroy(gameObject);

    yield return null;
  }

  void Update()
  {
    m_Duration -= Time.deltaTime;

    if (m_Duration > 0.0f)
    {
      // deal damage to blocks
      for (int h = 0; h < Height; ++h)
      {
        for (int i = 0; i < Game.I.gameRegion.Size.X; ++i)
        {
          var block = Game.I.HouseGrid[new IVector2(i, h) + Game.I.gameRegion.from];
          if (block != null)
          {
            // damage the block
          }
        }
      }
    }
    else
    {
      StartCoroutine(EndCoroutine());
    }
  }
}
