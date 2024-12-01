using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flooding : MonoBehaviour
{
    protected Block _block;
    Material _material;

    public float _flooding;

    public float waterCapacity = 1f;


    Vector3 rayFrom;
    Vector3 rayTo;
    public float waveAmount = 1f;


    public float FloodingAmount
    {
        get => FloodingPercent * waterCapacity;
        set
        {
            FloodingPercent = value / waterCapacity;
        }
    }

    public float FloodingPercent
    {
        get => _flooding;
        set
        {
            _flooding = value;
            _material.SetFloat("_Flood", _flooding);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _block = transform.parent.gameObject.GetComponent<Block>();
        _material = GetComponent<SpriteRenderer>().material;
        FloodingAmount = _flooding;
        rayFrom = _material.GetVector("_RayFrom");
        rayTo = _material.GetVector("_RayTo");
    }

    void Update() => MyUpdate();

    protected virtual void MyUpdate()
    {
        if (FloodingPercent >= 1f)
            _block.DealDamage(Time.deltaTime * 20f);
        waveAmount *= Mathf.Pow(0.4f, Time.deltaTime);
        float balance = waveAmount * Mathf.Sin(Time.time * 6f);
        _material.SetVector("_RayFrom", new Vector3(rayFrom.x, rayFrom.y * Mathf.Exp(balance), rayFrom.z));
        _material.SetVector("_RayTo", new Vector3(rayTo.x, rayTo.y / Mathf.Exp(balance), rayTo.z));
    }
}
