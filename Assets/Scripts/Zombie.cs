using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
  [SerializeField]
  public float DamagePerSecond = 5.0f;

  float m_RaycastDist = 0.0f;

  void Start()
  {
    m_RaycastDist = UnityEngine.Random.Range(0.05f, 0.25f);
  }

  void Update()
  {
    Block attack = null;

    var hits = Physics2D.RaycastAll(transform.position.ToVec2() + new Vector2(0.0f, 0.5f), Vector2.left, m_RaycastDist);
    foreach (var hit in hits)
    {
      if (hit.collider.TryGetComponent<Block>(out Block b))
      {
        attack = b;
        break;
      }
    }

    if (attack == null)
    {
      // move
      transform.position += new Vector3(-1.0f, 0.0f, 0.0f) * Time.deltaTime;
    }
    else
    {
      // attack
      attack.DealDamage(DamagePerSecond * Time.deltaTime);
    }
  }
}
