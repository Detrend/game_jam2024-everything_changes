using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxConveyorBeltControler : MonoBehaviour
{
    private Vector2 screenBounds;

    public static Vector2 MouseWorldPos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
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
        // destroy object when y is outside of the screen
        if( transform.position.y < -screenBounds.y - 1.0f)
        {
            Destroy(gameObject, 0.5f);
        }
    }
    /// <summary>
    /// Gets called when player starts dragging the box - set the box to be kinematic, freeze rotation and set the box to be the selected box
    /// </summary>
    public void Selected()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
        transform.rotation = Quaternion.identity;
        // remove this script from the box
        Destroy(this);
    }
}
