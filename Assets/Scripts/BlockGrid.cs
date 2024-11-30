
using System.Collections.Generic;
using UnityEngine;

public class BlockGrid : MonoBehaviour
{
    protected List<Block> _allBlocks;
    public List<Block> AllBlocks => _allBlocks;


    BlockRecord[,] data;

    public BBox BBox;

    public BlockRecord this[IVector2 pos]
    {
        get => InBounds(pos) ? data[pos.X - BBox.from.X, pos.Y - BBox.from.Y] : null;
        set => data[pos.X - BBox.from.X, pos.Y - BBox.from.Y] = value;
    }

    void Start() => MyStart();

    // Start is called before the first frame update
    protected virtual void MyStart()
    {
        _allBlocks = new();
        data = new BlockRecord[BBox.Size.X, BBox.Size.Y];
    }



    public void AddBlockAt(Block block, IVector2 pos)
    {
        _allBlocks.Add(block);
        // TODO? Maybe allow resizing the grid beforehand? Might be more memory efficient and faster during init
        //if (!_BBox.Contains(block.BBox))
        //{
        //    BBox new_bbox = _BBox.Extend(block.BBox);
        //    BlockRecord[,] new_data = new BlockRecord[new_bbox.Size.X, new_bbox.Size.Y];
        //    CopyData(data, new_data, _BBox.From - new_bbox.From);
        //    _BBox = new_bbox;
        //}

        foreach (IVector2 dif in block.size.AllCoordinates)
        {
            this[pos + dif] = new BlockRecord(block, dif);
        }
    }

    public void RemoveBlockAt(IVector2 pos)
    {
        Block b = this[pos].block;
        _allBlocks.Remove(b);
        foreach (IVector2 c in b.BBox.AllCoordinates)
        {
            this[c] = null;
        }
    }

    public bool InBounds(IVector2 pos) => BBox.Contains(pos);

}
