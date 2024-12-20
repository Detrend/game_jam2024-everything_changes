using System.Collections.Generic;
using UnityEngine;






public struct BoxSearchData
{
    public bool visited;
    public int belong;
}


public enum BlockType
{
    SENTRY, ROOF, SOLID, HAIRDRIER, TOILET_PAPER
    , COUNT // keep count at the end 
}



public class Block : MonoBehaviour
{
    public BBox BBox;

    public IVector2 size;

    public bool canBeMoved = true;
    public bool canBeDamaged = true;
    public bool isSolid = false;

    private bool _grabbed;

    public BlockGrid _parentGrid = null;


    SpriteRenderer _spriteRenderer;
    Material _material;

    [SerializeField]
    public AudioClip GrabSfx;

    [SerializeField]
    public AudioClip DropSfx;

    [SerializeField]
    public AudioClip DmgSfx;

    AudioSource m_AudioSrc;

    List<Block> _blocksAbove;
    List<Block> _blocksBelow;

    public List<Block> BlocksAbove => _blocksAbove;
    public List<Block> BlocksBelow => _blocksBelow;


    public BoxSearchData boxSearchData;

    public BlockType blockType;



    Flooding _flooding;
    public Flooding Flooding => _flooding;


    float _HP = 100.0f;
    public float MaxHP = 100f;

    public SpriteRenderer foregroundSpriteRenderer;

    public float HP
    {
        get => _HP;
        private set
        {
            _HP = value;
            if (foregroundSpriteRenderer != null)
                foregroundSpriteRenderer.material.SetFloat("_Hp", _HP / MaxHP);
        }
    }


    public virtual void DealDamage(float dmg)
    {
        if (!canBeDamaged) return;
        HP = Mathf.Max(HP - dmg, 0f);
        if (m_AudioSrc && !m_AudioSrc.isPlaying && DmgSfx)
        {
          m_AudioSrc.PlayOneShot(DmgSfx);
        }
    }

    public virtual void DealWaterDamage(float amount)
    {
      
    }

    public virtual float GetWaterAmount()
    {
        return _flooding.FloodingAmount;
    }

    public virtual float GetMaxWaterAmount()
    {
        return _flooding.waterCapacity;
    }

    public void ReactToVelocityChange(float amount)
    {
        DealDamage(amount * 2f);
        if (_flooding!= null) _flooding.waveAmount = amount / 5f;
    }

    //private void OnDrawGizmos()
    //{
    //  float ratio = MaxHP > 0.0f ? HP / MaxHP : 1.0f;
    //  Gizmos.color = Color.green;
    //  Gizmos.DrawCube(transform.position, new Vector3(ratio, 0.25f, 0.0f));
    //}

  protected void Awake()
    {
        _blocksAbove = new();
        _blocksBelow = new();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
        _material.SetFloat("_BlockWidth", size.X);

        BBox = new(((Vector2)transform.position).ToIVec(), size);
        transform.position = BBox.from.ToVec();

        m_AudioSrc = GetComponent<AudioSource>();
    }


    private void Start()
    {
        _flooding = GetComponentInChildren<Flooding>();
        HP = HP;
    }

    public void Place(BlockGrid grid, IVector2 pos, bool search_above_below, bool create_falling_grid = true)
    {
        BBox = new BBox(pos, size);
        grid.AddBlockAt(this, pos);
        _parentGrid = grid;

        for (int child_i = 0; child_i < transform.childCount; child_i++)
            transform.GetChild(child_i).gameObject.SetActive(true);

        transform.position = BBox.from.ToVec();

        if (search_above_below && m_AudioSrc && DropSfx != null && grid == Game.I.HouseGrid)
        {
            m_AudioSrc.PlayOneShot(DropSfx);
        }
       
        //_material.SetInt("_InHouseGrid", _parentGrid == Game.I.HouseGrid ? 1 : 0);
        
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
        if (create_falling_grid)
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


    public void RemoveFromGridAndSever(bool create_falling_grid = true)
    {
        //_material.SetInt("_InHouseGrid", 0);
        if (_parentGrid != null)
            _parentGrid.RemoveBlockAt(BBox.from);
        _parentGrid=null;

        for (int child_i = 0; child_i < transform.childCount; child_i++)
            transform.GetChild(child_i).gameObject.SetActive(false);


        foreach (Block block_above in _blocksAbove)
            block_above.BlocksBelow.Remove(this);
        _blocksAbove.Clear();

        foreach (Block block_below in _blocksBelow)
            block_below.BlocksAbove.Remove(this);
        _blocksBelow.Clear();

        if (create_falling_grid)
            Game.I.HouseGrid.CreateFallingGridFromUnstable();
    }

    public void Grab()
    {
        if (!canBeMoved) return;

        if (m_AudioSrc && GrabSfx != null)
        {
          m_AudioSrc.PlayOneShot(GrabSfx);
        }

    RemoveFromGridAndSever();

        _grabbed = true;
        //_grabbedPart = Game.MouseWorldPos.ToIVec() - transform.position.ToIVec();
        _material.SetInt("_Ghost", 1);
        _spriteRenderer.sortingOrder = 100;
    }

    protected void Update()
    {
        if (HP == 0f)
        {
            if (_parentGrid != null)
            {
                RemoveFromGridAndSever();
                var rb = gameObject.GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                _spriteRenderer.sortingOrder = -10;
                rb.excludeLayers = LayerMask.GetMask("Default");
            }
            else
            {
                if (transform.position.y < -20) Destroy(this);
            }
            
        }


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
