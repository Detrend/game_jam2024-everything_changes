using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletDrop : MonoBehaviour
{
  [SerializeField]
  public float Lifetime = 10.0f;

  float m_Lifetime = 0.0f;

  Vector2 m_Velocity = Vector2.zero;
  float m_Rot = 0.0f;
  float m_Rate = 0.0f;

  void Start()
  {
    m_Lifetime = Lifetime;
    m_Velocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(2.5f, 5.0f));
    m_Rate = Random.Range(-90.0f, 90.0f);
  }

  void Update()
  {
    m_Rot += m_Rate * Time.deltaTime;

    m_Lifetime -= Time.deltaTime;
    m_Velocity += new Vector2(0.0f, -10.0f) * Time.deltaTime;
    transform.position += (Vector3)m_Velocity * Time.deltaTime;
    var angles = transform.rotation.eulerAngles;
    transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_Rot));

    if (m_Lifetime <= 0.0f)
    {
      Destroy(gameObject);
    }
  }
}
