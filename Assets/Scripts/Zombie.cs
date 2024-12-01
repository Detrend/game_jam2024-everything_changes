using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
  [SerializeField]
  public float DamagePerSecond = 5.0f;

  [SerializeField]
  public float DamageShineTime = 0.2f;

  [SerializeField]
  public float MaxHP = 100.0f;

  float m_RaycastDist = 0.0f;
  float m_HP          = 0.0f;
  float m_JustDamaged = 0.0f;

  void Start()
  {
    m_RaycastDist = UnityEngine.Random.Range(0.05f, 0.25f);
    m_HP          = MaxHP;
  }

  public void DealDamage(float dmg)
  {
    m_HP -= dmg;
    m_HP = Mathf.Max(m_HP, 0.0f);
    m_JustDamaged = DamageShineTime;
  }

  private void OnDrawGizmos()
  {
    float max = MaxHP;
    float hp  = m_HP;
    float ratio = max > 0.0f ? hp / max : 1.0f;
    Gizmos.color = Color.green;
    Gizmos.DrawCube(transform.position, new Vector3(ratio, 0.25f, 0.0f));
  }

  void Update()
  {
    if (m_HP <= 0.0f)
    {
      GameObject.Destroy(gameObject);
      return;
    }

    m_JustDamaged = Mathf.Max(m_JustDamaged - Time.deltaTime, 0.0f);

    if (m_JustDamaged > 0.0f)
    {
      // shine
    }

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
