using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBlockFlooding : Flooding
{
    public float floodingLimit = 0.3f;

    public float volumePerParticle = 0.01f;
    float _currentParticleProgress = 0f;

    protected override void MyUpdate()
    {
        base.MyUpdate();
        if (FloodingAmount > floodingLimit)
        {

            float delta = 0.04f * Time.deltaTime * (FloodingAmount - floodingLimit);
            

            if (Game.CanPlaceBlockAt(_block, _block.BBox.from + new IVector2(1, 0)))
            {
                FloodingAmount -= delta;
                //RaycastHit2D hit = Physics2D.Raycast(_block.BBox.from.ToVec() + new Vector2(1.5f, 0.5f), new Vector2(0, -1));

                //float time = 1f;

                //if (hit.collider != null && hit.collider.CompareTag("Block"))
                //{
                //    time = Mathf.Sqrt(2 * hit.distance / Physics2D.gravity.y);
                //    Block b2 = hit.collider.gameObject.GetComponent<Block>();
                //    if (b2.blockType != BlockType.ROOF)
                //    {
                //        hit.collider.gameObject.GetComponent<Flooding>().FloodingAmount += 0.2f * Time.deltaTime;
                //    }
                //}
                _currentParticleProgress += delta;
                if (_currentParticleProgress > volumePerParticle)
                {
                    _currentParticleProgress -= volumePerParticle;
                    ParticleSystem.EmitParams ps = new()
                    {
                        position = transform.position + new Vector3(.5f, FloodingAmount * 0.7f - 0.3f),
                        velocity = new Vector3(Random.Range(0.1f, 0.4f), 0f, 0f)
                        //startLifetime = time
                    };
                    Game.I.floodParticles.Emit(ps, 1);
                }
            }
        }
    }
}
