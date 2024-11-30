using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;





public class ScreenShake
{
    public float _amount;
    readonly float _decay;

    public List<(Vector2, float)> _shakeVectors;

    public ScreenShake(float decay)
    {
        _amount = 0f;
        _decay = decay;
        _shakeVectors = new();
    }


    public Vector2 Update()
    {
        _amount *= Mathf.Pow(_decay, Time.deltaTime);

        Vector2 dif = Vector2.zero;
        foreach ((Vector2 t, float move) in _shakeVectors)
        {
            dif += Mathf.Sin(move * Time.time) * t;
        }
        return _amount * dif;
    }
}






public class ScreenShaker : MonoBehaviour
{
    List<ScreenShake> _shakes = new();
    Vector3 m_InitialCameraPos = Vector3.zero;

    void Awake()
    {
        m_InitialCameraPos = Camera.main.transform.position;
    }

    public void AddShake(ScreenShake shake) => _shakes.Add(shake);


      void Update()
      {
            //m_Shakes.RemoveAll(item => item.time <= 0.0f);

            //float max = 0.0f;

            //amount = amount * Mathf.Pow(0.4f, Time.deltaTime);

            // update the shakes
            //foreach (var shake in m_Shakes)
            //{
            //  shake.time -= Time.deltaTime;
            //  max         = Mathf.Max(max, shake.amount * shake.time / shake.maxTime);
            //}

            Vector2 dif = Vector2.zero;
            foreach (ScreenShake s in _shakes) dif += s.Update();

            //float spd = 1.5f;

            //var axis1 = new Vector3(0.7f, 0.3f, 0.0f) * (Mathf.Sin(Time.time * 25.2f * spd) * amount);
            //var axis2 = new Vector3(-0.1f, 0.5f, 0.0f) * (Mathf.Sin(Time.time * 34.2f * spd) * amount);
            //var axis3 = new Vector3(-0.2f, 0.5f, 0.0f) * Mathf.Sin(Time.time * 18.2f * spd) * max * 0.2f;
            //var axis4 = new Vector3(0.4f, -0.3f, 0.0f) * Mathf.Sin(Time.time * 13.2f * spd) * max * 0.1f;
            //var axis5 = new Vector3(0.1f, 0.5f, 0.0f)  * Mathf.Sin(Time.time * 20.0f * spd) * max;

            Camera.main.transform.position = m_InitialCameraPos + (Vector3) dif;// + axis3 + axis4 + axis5) * 0.2f;
      }
}
