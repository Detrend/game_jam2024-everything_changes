using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game I { get; private set; }

    [NonSerialized] public HouseGrid       HouseGrid;
    [NonSerialized] public DisasterManager DisasterManager;
    [NonSerialized] public DisasterSpawner DisasterSpawner;
    [NonSerialized] public ScreenShaker    ScreenShaker;

    public List<FallingGrid> FallingGrids;

    public GameObject fallingGridPrefab;


    public BBox gameRegion;


    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        else I = this;

        HouseGrid = GetComponentInChildren<HouseGrid>();
        DisasterManager = GetComponentInChildren<DisasterManager>();
        DisasterSpawner = GetComponentInChildren<DisasterSpawner>();
        ScreenShaker    = GetComponentInChildren<ScreenShaker>();
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

    private void OnDestroy()
    {
        I = null;
    }
    
    public static bool CanPlaceBlockAt(Block block, IVector2 pos)
    {
        if (!I.HouseGrid.CanPlaceBlockAt(block, pos)) return false;
        foreach (FallingGrid g in I.FallingGrids)
        {
            if (!g.CanPlaceBlockAt(block, pos)) return false;
        }
        return true;
    }

    public static IVector2 ClosestValidPosition(Block b, Vector2 pos)
    {
        if (CanPlaceBlockAt(b, pos.ToIVec())) return pos.ToIVec();

        float best_dist = 10000;
        IVector2 best_placement = IVector2.Zero;

        foreach (IVector2 p in I.gameRegion.AllCoordinates)
        {
            if (CanPlaceBlockAt(b, p))
            {
                float dist = IVector2.DistanceL2(p, pos);
                if (dist < best_dist)
                {
                    best_dist = dist;
                    best_placement = p;
                }
            }
        }

        if (best_dist == 10000)
        {
            Debug.LogError("Cannot place block: No valid space in the entire grid!");
            return IVector2.Zero;
        }
        return best_placement;
    }






    public static Vector2 MouseWorldPos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public static IVector2 MouseTilePos => MouseWorldPos.ToIVec();

    public static bool InBounds(IVector2 pos) => I.gameRegion.Contains(pos);
}
