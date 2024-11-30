using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  void Start()
  {
    float startX = (float)(Game.I.gameRegion.from.X + Game.I.gameRegion.to.X) * 0.5f;
    float startY = Game.I.gameRegion.to.Y;

    m_MoveAway    = Game.I.transform.position + new Vector3(startX, startY + 8.0f, 0.0f);
    m_MoveTowards = Game.I.transform.position + new Vector3(startX, startY + 1.0f, 0.0f);

    transform.position = m_MoveAway;

    m_RemainTime = RemainTime;
  }

  void DoAttack()
  {
    
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
          GameObject.Destroy(gameObject);
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
