using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
  public void ShakeScreen(float amount, float time)
  {
    m_Shakes.Add(new Shake(amount, time));
  }

  class Shake
  {
    public float amount;
    public float time;
    public float maxTime;

    public Shake(float a, float t)
    {
      amount  = a;
      time    = t;
      maxTime = t;
    }
  }

  List<Shake> m_Shakes = new List<Shake>();

  Vector3 m_InitialCameraPos = Vector3.zero;

  void Start()
  {
    m_InitialCameraPos = Camera.main.transform.position;
  }

  void Update()
  {
    m_Shakes.RemoveAll(item => item.time <= 0.0f);

    float max = 0.0f;

    // update the shakes
    foreach (var shake in m_Shakes)
    {
      shake.time -= Time.deltaTime;
      max         = Mathf.Max(max, shake.amount * shake.time / shake.maxTime);
    }

    float spd = 1.5f;

    var axis1 = new Vector3(0.7f, 0.3f, 0.0f)  * Mathf.Sin(Time.time * 25.2f * spd) * max;
    var axis2 = new Vector3(0.1f, 0.8f, 0.0f)  * Mathf.Sin(Time.time * 29.2f * spd) * max;
    var axis3 = new Vector3(-0.2f, 0.5f, 0.0f) * Mathf.Sin(Time.time * 18.2f * spd) * max * 0.2f;
    var axis4 = new Vector3(0.4f, -0.3f, 0.0f) * Mathf.Sin(Time.time * 13.2f * spd) * max * 0.1f;
    var axis5 = new Vector3(0.1f, 0.5f, 0.0f)  * Mathf.Sin(Time.time * 20.0f * spd) * max;

    Camera.main.transform.position = m_InitialCameraPos + (axis1 + axis2 + axis3 + axis4 + axis5) * 0.2f;
  }
}
