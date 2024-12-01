using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodPrefab : MonoBehaviour
{
    ParticleSystem _ps;


    float current_width = 0f;
    float remaining_duration = 7;
    public int maxWidth = 3;
    public float floodingStrength = 0.3f;
    float distanceX;
    float speedX;
    float startTime;


    ParticleSystem.Particle[] _particles;

    // Start is called before the first frame update
    void Start()
    {
        _ps = GetComponent<ParticleSystem>();
        distanceX = transform.position.x;
        speedX = -_ps.velocityOverLifetime.x.constant;
        startTime = Time.time;
        _particles = new ParticleSystem.Particle[_ps.main.maxParticles];
    }

    // Update is called once per frame
    void Update()
    {
        remaining_duration -= Time.deltaTime;
        if (remaining_duration > 0f)
        {
            current_width = Mathf.Min(1f, current_width + 0.4f * Time.deltaTime);
        }
        else
        {
            current_width = Mathf.Max(0f, current_width - 0.4f * Time.deltaTime);
        }

        var emm = _ps.emission;
        emm.rateOverTime = (int) (current_width * maxWidth * 200); 


        var sh = _ps.shape;
        sh.position = new Vector3(15, -2 + maxWidth * current_width / 2, 0);
        sh.scale = new Vector3(1f, maxWidth * current_width, 1f);

        int num_particles = _ps.GetParticles(_particles);

        int write_i = 0;
        for (int i = 0; i < num_particles; i++)
        {
            
            IVector2 ip = _particles[i].position.ToIVec();
            BlockRecord br = Game.I.HouseGrid[ip];
            if (br != null)
            {
                Flooding f = br.block.Flooding;
                if (f != null)
                {
                    f.FloodingAmount += floodingStrength;
                }
            }
            else
            {
                _particles[write_i++] = _particles[i];
            }
        }
        _ps.SetParticles(_particles, write_i);

        if (num_particles == 0 && remaining_duration < 0f) Destroy(this);
    }
}
