using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;






public struct BoxSearchData
{
    public bool visited;
}


public enum BlockType
{
    ROOF, SENTRY, TOILET_PAPER
}



public class Block : MonoBehaviour
{
    BBox _BBox;

    public BBox BBox => _BBox;

    public IVector2 size;

    public bool canBeMoved = true;

    private bool _grabbed;
    //private IVector2 _grabbedPart;

    private BlockGrid _parentGrid = null;


    SpriteRenderer _spriteRenderer;
    Material _material;


    List<Block> _blocksAbove;
    List<Block> _blocksBelow;

    public List<Block> BlocksAbove => _blocksAbove;
    public List<Block> BlocksBelow => _blocksBelow;


    public BoxSearchData boxSearchData;

    public BlockType blockType;



    Flooding _flooding;


    float m_HP = 100.0f;

    public virtual void DealDamage(float dmg)
    {
      m_HP -= dmg;
      m_HP = Mathf.Max(m_HP, 0.0f);
    }

    public virtual float GetHP()
    {
        return m_HP;
    }

    public virtual float GetMaxHP()
    {
        return 100.0f;
    }

    public virtual void DealWaterDamage(float amount)
    {
      
    }

    public virtual bool IsSolid()
    {
        return false;
    }

  public virtual float GetWaterAmount()
    {
        return 0.0f;
    }

    public virtual float GetMaxWaterAmount()
    {
        return 0.0f;
    }

    private void OnDrawGizmos()
    {
      float max = GetMaxHP();
      float hp  = GetHP();
      float ratio = max > 0.0f ? hp / max : 1.0f;
      Gizmos.color = Color.green;
      Gizmos.DrawCube(transform.position, new Vector3(ratio, 0.25f, 0.0f));
    }

  protected void Awake()
    {
        _blocksAbove = new();
        _blocksBelow = new();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
        _material.SetFloat("_BlockWidth", size.X);

        _BBox = new(((Vector2)transform.position).ToIVec(), size);
        transform.position = _BBox.from.ToVec();

        _flooding = GetComponentInChildren<Flooding>();
    }

    public void Place(BlockGrid grid, IVector2 pos, bool search_above_below)
    {
        _BBox = new BBox(pos, size);
        grid.AddBlockAt(this, pos);
        _parentGrid = grid;

        transform.position = _BBox.from.ToVec();
       
        _material.SetInt("_InHouseGrid", _parentGrid == Game.I.HouseGrid ? 1 : 0);
        
        if (search_above_below)
        {
            for (int x = BBox.from.X; x < BBox.to.X; x++)
            {
                BlockRecord block_below = grid[new IVector2(x, BBox.from.Y - 1)];
                if (block_below != null && !_blocksBelow.Contains(block_below.block)) LinkWithBelow(block_below.block);

                BlockRecord block_above = grid[new IVector2(x, BBox.to.Y)];
                if (block_above != null && !_blocksAbove.Contains(block_above.block)) LinkWithAbove(block_above.block);
            }
        }
        Game.I.HouseGrid.CreateFallingGridFromUnstable();
    }

    public void RemoveFromGrid()
    {
        _parentGrid.RemoveBlockAt(BBox.from);
        _parentGrid = null;
    }

    public void LinkWithBelow(Block b) {
        _blocksBelow.Add(b);
        b._blocksAbove.Add(this);
    }
    public void LinkWithAbove(Block b) {
        _blocksAbove.Add(b);
        b._blocksBelow.Add(this);
    }

    public void RemoveLinkWithBelow(Block b)
    {
        _blocksBelow.Remove(b);
        b._blocksAbove.Remove(this);
    }
    public void RemoveLinkWithAbove(Block b)
    {
        _blocksAbove.Remove(b);
        b._blocksBelow.Remove(this);
    }




    public void Grab()
    {
        if (!canBeMoved) return;

        _material.SetInt("_InHouseGrid", 0);
        if (_parentGrid != null)
            _parentGrid.RemoveBlockAt(BBox.from);


        foreach (Block block_above in _blocksAbove)
            block_above.BlocksBelow.Remove(this);
        _blocksAbove.Clear();

        foreach (Block block_below in _blocksBelow)
            block_below.BlocksAbove.Remove(this);
        _blocksBelow.Clear();

        Game.I.HouseGrid.CreateFallingGridFromUnstable();

        _grabbed = true;
        //_grabbedPart = Game.MouseWorldPos.ToIVec() - transform.position.ToIVec();
        _material.SetInt("_Ghost", 1);
        _spriteRenderer.sortingOrder = 100;
    }

    protected void Update()
    {
        foreach (Block above in BlocksAbove)
        {
            Vector2 a = above.BBox.Center, b = BBox.Center;
            Debug.DrawLine(Vector2.Lerp(a, b, 0.25f), Vector2.Lerp(a, b, 0.75f));
        }


        if (_grabbed)
        {
            Vector2 cur_pos = Game.MouseWorldPos - BBox.Size.ToVec() / 2 + Vector2.one * 0.5f;
            IVector2 int_pos = cur_pos.ToIVec();

            bool valid_placement = Game.CanPlaceBlockAt(this, int_pos);
            _material.SetInt("_PlacementValid", 1);// valid_placement ? 1 : 0);
            IVector2 place_pos = Game.ClosestValidPosition(this, cur_pos);
            transform.position = place_pos.ToVec();

            if (Input.GetMouseButtonUp(0))
            {
                transform.position = place_pos.ToVec();
                Place(Game.I.HouseGrid, place_pos, true);
                
                _grabbed = false;
                _material.SetInt("_Ghost", 0);
                _spriteRenderer.sortingOrder = 0;
            }
        }
    }
}
