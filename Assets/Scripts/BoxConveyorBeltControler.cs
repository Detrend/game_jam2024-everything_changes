using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxConveyorBeltControler : MonoBehaviour
{
    public static Vector2 MouseWorldPos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit_collider = Physics2D.OverlapPoint(MouseWorldPos);
            if (hit_collider != null && hit_collider.gameObject == this.gameObject)
            {
                Selected();
            }
        }
    }
    /// <summary>
    /// Gets called when player starts dragging the box - set the box to be kinematic, freeze rotation and set the box to be the selected box
    /// </summary>
    public void Selected()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        // remove this script from the box
        Destroy(this);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject, 0.25f);
    }
}
