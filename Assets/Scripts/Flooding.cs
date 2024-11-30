using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flooding : MonoBehaviour
{
    protected Block _block;
    Material _material;

    public float _flooding;
    public float FloodingAmount
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
    }
}
