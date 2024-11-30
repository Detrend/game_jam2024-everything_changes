using JetBrains.Annotations;
using UnityEngine;





public class BlockRecord
{
    readonly public Block block;
    readonly public IVector2 offset;

    public BlockRecord(Block b, IVector2 offset)
    {
        block = b;
        this.offset = offset;
    }
}




public class HouseGrid : MonoBehaviour
{
    BlockRecord[,] data;

    public BBox _BBox;



    public BlockRecord this[IVector2 pos]
    {
        get => data[pos.X - _BBox.From.X, pos.Y - _BBox.From.Y];
        set => data[pos.X - _BBox.From.X, pos.Y - _BBox.From.Y] = value;
    }


    void Start()
    {
        data = new BlockRecord[20, 40];
        _BBox = new BBox(new (-10, -10), new(20, 40));
    }

    public void AddBlock(Block block, IVector2 pos)
    {
        if (!BlockFits(block, pos))
        {
            Debug.Log("Cannot Add block. Doesn't Fit.");
            return;
        }
        block.Place(pos);
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
        foreach (IVector2 c in b.BBox.AllCoordinates)
        {
            this[c] = null;
        }
    }

    public bool BlockFits(Block block, IVector2 pos)
    {
        for (int i = 0; i < block.size.X; i++)
        {
            for (int j = 0; j < block.size.Y; j++)
            {
                IVector2 check = new IVector2(i, j) + pos;
                if (!_BBox.Contains(check) || this[check] != null) return false;
            }
        }
        return true;
    }

    private static void CopyData(BlockRecord[,] src, BlockRecord[,] dst, IVector2 dst_pos)
    {
        for (int i = 0; i < src.GetLength(0); i++)
        {
            for (int j = 0; j < src.GetLength(1); j++)
            {
                dst[i + dst_pos.X, j + dst_pos.Y] = dst[i, j];
            }
        }
    }
}
