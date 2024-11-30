using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foundation : Block
{
    public override void DealDamage(float dmg)
    {
      
    }

    public override float GetHP()
    {
      return 100.0f;
    }

    public override float GetMaxHP()
    {
        return 100.0f;
    }

    public override void DealWaterDamage(float amount)
    {
      
    }

    public override float GetWaterAmount()
    {
        return 0.0f;
    }

    public override float GetMaxWaterAmount()
    {
        return 100.0f;
    }
}
