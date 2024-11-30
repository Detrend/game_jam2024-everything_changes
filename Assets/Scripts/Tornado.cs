using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
  [SerializeField]
  public float MovementSpeed = 1.0f;

  [SerializeField]
  public float MaxHP = 100.0f;

  [SerializeField]
  public float HPStopThreshold = 20.0f;

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
  float        m_HP    = 0.0f;

  void Start()
  {
    m_HP   = MaxHP;
    m_Xmin = Game.I.HouseGrid.BBox.from.X + Game.I.HouseGrid.transform.position.x;
    m_Xmax = Game.I.HouseGrid.BBox.to.X   + Game.I.HouseGrid.transform.position.x;
  }

  void Update()
  {
    float dir = m_State == TornadoState.MovingLeft ? -1.0f : 1.0f;
    transform.position += new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * MovementSpeed * dir;

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

    if (m_HP < HPStopThreshold && m_State != TornadoState.Standing)
    {
      m_State = TornadoState.Standing;
    }
  }
}
