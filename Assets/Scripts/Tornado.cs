using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
  [SerializeField]
  public float MovementSpeed = 1.0f;

  [SerializeField]
  public float DamagePerSecond = 25.0f;

  [SerializeField]
  public float HorizontalRange = 3.0f;

  [SerializeField]
  public float Duration = 15.0f;

  [SerializeField]
  public float HPStopThreshold = 20.0f;

  [SerializeField]
  public float TurbineDmg = 0.5f;

  enum TornadoState
  {
    MovingLeft,
    MovingRight,
    Standing
  }

  // Tornado will move between these two X positions
  float        m_Xmin  = 0.0f;
  float        m_Xmax  = 0.0f;
  TornadoState m_State = TornadoState.MovingLeft;
  float        m_Duration = 0.0f;

  void Start()
  {
    m_Duration = Duration;
    m_Xmin = Game.I.gameRegion.from.X + Game.I.HouseGrid.transform.position.x;
    m_Xmax = Game.I.gameRegion.to.X   + Game.I.HouseGrid.transform.position.x;
  }

  void DealDmg()
  {
    for (int x = Game.I.gameRegion.from.X; x <= Game.I.gameRegion.to.X; x++)
    {
      Block top = null;

      for (int y = Game.I.gameRegion.from.Y; y <= Game.I.gameRegion.to.Y; y++)
      {
        var b = Game.I.HouseGrid[new IVector2(x, y)];
        if (b != null)
        {
          top = b.block;
          if (b.block.TryGetComponent(out Turbine t))
          {
            m_Duration -= TurbineDmg * Time.deltaTime;
          }
        }
      }

      // dmg to the top block
      if (top)
      {
        float center = top.transform.position.x + top.BBox.Size.X * 0.5f;
        float xdiff = Mathf.Abs(transform.position.x - center);
        if (xdiff < HorizontalRange)
        {
          top.DealDamage(DamagePerSecond * Time.deltaTime);
        }
      }
    }
  }

  void Update()
  {
    if (m_State != TornadoState.Standing)
    {
      float dir = m_State == TornadoState.MovingLeft ? -1.0f : 1.0f;
      transform.position += new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * MovementSpeed * dir;
    }

    m_Duration -= Time.deltaTime;

    if (m_State == TornadoState.MovingLeft && transform.position.x <= m_Xmin)
    {
      m_State = TornadoState.MovingRight;
      transform.position = new Vector3(m_Xmin, transform.position.y, transform.position.z);
    }
    else if (m_State == TornadoState.MovingRight && transform.position.x >= m_Xmax)
    {
      m_State = TornadoState.MovingLeft;
      transform.position = new Vector3(m_Xmax, transform.position.y, transform.position.z);
    }

    if (m_Duration < HPStopThreshold && m_State != TornadoState.Standing)
    {
      m_State = TornadoState.Standing;
    }

    DealDmg();

    if (m_Duration <= 0.0f)
    {
      GameObject.Destroy(gameObject);
    }
  }
}
