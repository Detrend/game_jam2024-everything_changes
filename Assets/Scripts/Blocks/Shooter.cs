using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Block
{
  [SerializeField]
  public float ShootingRange = 4.0f;

  [SerializeField]
  public float ShootingDegree = 45.0f;

  [SerializeField]
  public float DamagePerHit = 25.0f;

  [SerializeField]
  public float ReloadTime = 0.5f;

  float m_Reload      = 0.0f;
  float m_RotateAngle = 0.0f;

  GameObject m_Gun = null;

  private void Awake()
  {
    base.Awake();
    m_Gun = transform.Find("Gun")?.gameObject;
  }

  private void OnDrawGizmosSelected()
  {
    int iterations = 12;
    for (int i = 0; i < iterations; ++i)
    {
      float coeff  = i / (float)iterations;
      float coeffn = (i + 1) / (float)iterations;
      float angle  = -ShootingDegree * coeff  + (1.0f - coeff)  * ShootingDegree;
      float anglen = -ShootingDegree * coeffn + (1.0f - coeffn) * ShootingDegree;

      Vector3 dira = Quaternion.AngleAxis(angle,  Vector3.forward) * Vector3.right * ShootingRange;
      Vector3 dirb = Quaternion.AngleAxis(anglen, Vector3.forward) * Vector3.right * ShootingRange;
      var offset = transform.position + new Vector3(BBox.Size.X, BBox.Size.Y, 0.0f) * 0.5f;
      Gizmos.DrawLine(offset + dira, offset + dirb);

      if (i == 0)
      {
        Gizmos.DrawLine(offset, offset + dira);
      }
      else if (i == iterations - 1)
      {
        Gizmos.DrawLine(offset, offset + dirb);
      }
    }
  }

  private new void Update()
  {
    base.Update();

    if (m_Reload > 0.0f)
    {
      m_Reload = Mathf.Max(0.0f, m_Reload - Time.deltaTime);
    }

    var zombies = GameObject.FindGameObjectsWithTag("Zombie");

    float  closestDist = float.MaxValue;
    Zombie target      = null;
    float  bestAngle   = 0.0f;

    Vector2 center = transform.position.ToVec2() + new Vector2(BBox.Size.X, BBox.Size.Y) * 0.5f;

    foreach (var zombie in zombies)
    {
      var zombiepos = zombie.transform.position.ToVec2() + new Vector2(0.0f, 0.5f);
      var dist  = Vector2.Distance(zombiepos, center);
      var dir   = (zombiepos - center).normalized;
      var angle = Vector3.Angle(Vector3.right, new Vector3(dir.x, dir.y, 0.0f));
      if (dist < ShootingRange
        && Mathf.Abs(angle) <= ShootingDegree
        && dist < closestDist
        && zombie.TryGetComponent(out Zombie zc))
      {
        target      = zc;
        closestDist = dist;
        bestAngle   = angle;
      }
    }

    if (target)
    {
      var pos = target.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
      m_Gun.transform.rotation = Quaternion.AngleAxis(-bestAngle, Vector3.forward);
    }
    else
    {
      m_Gun.transform.rotation = Quaternion.identity;
    }

    // get best possible target
    if (m_Reload == 0.0f && target != null)
    {
      // choose best possible target
      target.DealDamage(DamagePerHit);
      m_Reload = ReloadTime;
    }
  }
}
