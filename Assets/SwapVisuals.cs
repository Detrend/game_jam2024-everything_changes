using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SwapVisuals : MonoBehaviour
{
    public GameObject from;
    public GameObject to;

    float time;

    Material mat_from;
    Material mat_to;


    private void Start()
    {
        mat_from = from.GetComponent<SpriteRenderer>().material;
        mat_to = to.GetComponent<SpriteRenderer>().material;
    }


    public void Initialize(BBox a, BBox b)
    {
        from.transform.position = a.from.ToVec();
        from.transform.localScale = b.Size.ToVec();
        to.transform.position = b.from.ToVec();
        to.transform.localScale = b.Size.ToVec();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        mat_to.SetFloat("_Reveal", 1f - 2 * time);
        mat_from.SetFloat("_Reveal", 1f - 2 * time);
        
        if (1f - 2 * time < 0) Destroy(gameObject);
    }
}
