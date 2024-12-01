using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour
{
  [SerializeField]
  public int Range = 2;

  [SerializeField]
  public int DrainAmountPerSecond = 10;

  void Start()
  {
    
  }

  void Update()
  {
    // update the flooding of neighbor blocks
    var block = GetComponent<Block>();
    if (!block)
    {
      return;
    }

    if (!block._parentGrid)
    {
      return;
    }

    var bbox = block.BBox;
    for (int x = bbox.from.X - Range; x <= bbox.to.X + Range; ++x)
    {
      for (int y = bbox.from.Y - Range; y <= bbox.to.Y + Range; ++y)
      {
        var coords = new IVector2(x, y);
        if (Game.InBounds(coords))
        {
          // heal the block
          var other = Game.I.HouseGrid[coords];
          if (other != null)
          {
            other.block.Flooding.FloodingAmount -= DrainAmountPerSecond * Time.deltaTime;
          }
        }
      }
    }

  }
}
