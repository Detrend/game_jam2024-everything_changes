using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCloud : MonoBehaviour
{
    ParticleSystem _particleSystem;

    float _rainProgress = 0f;
    public float traceFrequency = 0.01f;
    public float rainTraceAmount = 0.1f;

    Vector2 tangent;

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        transform.position = new(30f, 10f, 0f);

        float angle = Random.Range(-0.25f, 0.25f);

        tangent = new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle));

        float speed = 12f;
        var vel = _particleSystem.velocityOverLifetime;
        vel.x = new ParticleSystem.MinMaxCurve(speed * tangent.x);
        vel.y = new ParticleSystem.MinMaxCurve(speed * tangent.y);

        transform.position = new Vector3(15, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        _rainProgress += Time.deltaTime;
        while (_rainProgress >= traceFrequency)
        {
            _rainProgress -= traceFrequency;
            Vector2 start_pos = new(transform.position.x + Random.Range(-10f, 10f), transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(start_pos, tangent);
            if (hit.collider != null && hit.collider.CompareTag("Block"))
            {
                Block b = hit.collider.GetComponent<Block>();
                if (b != null && b.Flooding != null)
                {
                    b.Flooding.FloodingAmount += rainTraceAmount;
                }
            }
        }

        Vector3 p = transform.position;
        transform.position = new Vector3(p.x - 1f * Time.deltaTime, p.y, p.z);

        if (p.x < -30f) Destroy(gameObject);
    }
}
