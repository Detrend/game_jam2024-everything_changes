using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
  [SerializeField]
  public float Duration = 10.0f;

  float m_Duration = 10.0f;

  void Start()
  {
    m_Duration = Duration;
  }

  void Update()
  {
    m_Duration -= Time.deltaTime;
    if (m_Duration < 0.0f)
    {
      GameObject.Destroy(gameObject);
    }

    for (int x = Game.I.gameRegion.from.X; x <= Game.I.gameRegion.to.X; x++)
    {
      Block top = null;

      for (int y = Game.I.gameRegion.from.X; y <= Game.I.gameRegion.to.X; y++)
      {
        var b = Game.I.HouseGrid[new IVector2(x, y)];
        if (b != null)
        {
          top = b.block;
        }
      }

      // water dmg to the top block
    }
  }
}
