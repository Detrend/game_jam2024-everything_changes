using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
  void Start()
  {
    
  }

  void Update()
  {
    transform.position += new Vector3(-1.0f, 0.0f, 0.0f) * Time.deltaTime;
  }
}
