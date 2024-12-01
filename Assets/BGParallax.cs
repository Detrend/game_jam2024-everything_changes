using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGParallax : MonoBehaviour
{

    public float parallaxCoeff;
    Vector3 p;

    // Start is called before the first frame update
    void Start()
    {
        p = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Game.I.ScreenShaker.Move * parallaxCoeff;
        transform.position = p + (Vector3) move;

    }
}
