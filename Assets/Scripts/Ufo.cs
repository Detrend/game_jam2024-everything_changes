using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Ufo : MonoBehaviour
{
  enum UfoStage
  {
    Incoming,
    Attacking,
    Leaving,
  }

  [SerializeField]
  public float RemainTime = 10.0f;

  [SerializeField]
  public float MoveSpeed = 10.0f;

  Vector3  m_MoveTowards = Vector3.zero;
  Vector3  m_MoveAway    = Vector3.zero;
  float    m_RemainTime  = 0.0f;
  UfoStage m_Stage       = UfoStage.Incoming;

    public float swapsPerSecond = 1f;
    float currentSwaps = 0f;


    void Start()
  {
    float startX = (Game.I.gameRegion.from.X + Game.I.gameRegion.to.X) * 0.5f;
    float startY = Game.I.gameRegion.to.Y;

    m_MoveAway    = Game.I.transform.position + new Vector3(startX, startY + 8.0f, 0.0f);
    m_MoveTowards = Game.I.transform.position + new Vector3(startX, startY + 1.0f, 0.0f);

    transform.position = m_MoveAway;

    m_RemainTime = RemainTime;
  }

    void DoAttack()
    {
        currentSwaps += swapsPerSecond * Time.deltaTime;
        if (currentSwaps < 1f) return;

        currentSwaps -= 1f;

        Block from = null;
        for (int i = 0; i < 100; i++)
        {
            IVector2 from_check = Game.RandomGridPosition();
            if (Game.I.HouseGrid[from_check] == null) continue;
            from = Game.I.HouseGrid[from_check].block;
        }
        if (from == null)
        {
            Debug.Log("Failed to find UFO from spot.");
            return;
        }

        Block to = null;
        for (int i = 0; i < 100; i++)
        {
            IVector2 to_check = Game.RandomGridPosition();
            if (Game.I.HouseGrid[to_check] == null) continue;

            Block temp = Game.I.HouseGrid[to_check].block;
            if (temp == from) continue;
            if (temp.BBox.Size != from.BBox.Size) continue;

            to = temp;
        }
        if (to == null)
        {
            Debug.Log("Failed to find UFO to spot.");
            return;
        }

        from.RemoveFromGridAndSever(false);
        to.RemoveFromGridAndSever(false);

        IVector2 ff = from.BBox.from;
        from.Place(Game.I.HouseGrid, to.BBox.from, true, false);
        to.Place(Game.I.HouseGrid, ff, true, false);

        GameObject g = Instantiate(Game.I.swapPrefab);
        g.GetComponent<SwapVisuals>().Initialize(from.BBox, to.BBox);

    }
  void Update()
  {
    if (m_Stage == UfoStage.Incoming || m_Stage == UfoStage.Leaving)
    {
      var dir = m_MoveTowards - transform.position;
      var add = dir.normalized * MoveSpeed * Time.deltaTime;
      transform.position += add;

      if (Vector3.Distance(transform.position, m_MoveTowards) < 0.2f)
      {
        if (m_Stage == UfoStage.Incoming)
        {
          transform.position = m_MoveTowards;
          m_Stage = UfoStage.Attacking;
        }
        else
        {
          Destroy(gameObject);
        }
      }
    }
    else if (m_Stage == UfoStage.Attacking)
    {
      m_RemainTime -= Time.deltaTime;

      DoAttack();

      if (m_RemainTime <= 0.0f)
      {
        m_Stage = UfoStage.Leaving;
        m_MoveTowards = m_MoveAway;
      }
    }
  }
}
