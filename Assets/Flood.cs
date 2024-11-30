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

  void Start()
  {
    m_Duration = Duration;
  }

  void Update()
  {
    m_Duration -= Time.deltaTime;

    if (m_Duration > 0.0f)
    {
      // deal damage to blocks
      for (int h = 0; h < Height; ++h)
      {
        for (int i = 0; i < Game.I.HouseGrid.BBox.Size.X; ++i)
        {
          var block = Game.I.HouseGrid[new IVector2(i, h) + Game.I.HouseGrid.BBox.from];
          if (block != null)
          {
            // damage the block
          }
        }
      }
    }
    else
    {
      GameObject.Destroy(gameObject);
    }
  }
}
