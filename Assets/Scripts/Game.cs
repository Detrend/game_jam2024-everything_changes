using System.Collections;
using System.Collections.Generic;
using System.Web;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game I { get; private set; }

    public HouseGrid HouseGrid;


    private void Awake()
    {
        if (I != null && I != this) Destroy(this);
        else I = this;
    }

    private void Start()
    {
        HouseGrid = GetComponentInChildren<HouseGrid>();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit_collider = Physics2D.OverlapPoint(MouseWorldPos);
            if (hit_collider != null)
            {
                // Check if the clicked object is this object
                if (hit_collider.gameObject.CompareTag("Block"))
                {
                    hit_collider.gameObject.GetComponent<Block>().Grab();
                }
            }
        }
    }


    public static Vector2 MouseWorldPos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public static IVector2 MouseTilePos => MouseWorldPos.ToIVec();
}
