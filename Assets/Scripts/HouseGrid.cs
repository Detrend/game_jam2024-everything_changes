using System.Collections.Generic;
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





public static class Search
{
    public static int belongingTracker = 0;

    public static void ResetSearchFlags(IEnumerable<Block> blocks)
    {
        foreach (Block b in blocks) b.boxSearchData.visited = false;
    }

    public static void SetBelongingFlag(IEnumerable<Block> blocks, int value)
    {
        foreach (Block b in blocks)
            b.boxSearchData.belong = value;
    }


  public static void RunAboveBFS(Block from)
  {
    Queue<Block> queue = new();
    queue.Enqueue(from);
    from.boxSearchData.visited = true;

        while (queue.Count > 0)
        {
            Block b = queue.Dequeue();
            foreach (Block b2 in b.BlocksAbove)
            {
                if (!b2.boxSearchData.visited)
                {
                    b2.boxSearchData.visited = true;
                    queue.Enqueue(b2);
                }
            }
        }
    }


}





public class HouseGrid : BlockGrid
{

    Block _foundationBlock;
    public Block FoundationBlock => _foundationBlock;

    protected override void MyStart()
    {
        base.MyStart();

        _foundationBlock = GetComponentInChildren<Block>();
        _foundationBlock.Place(Game.I.HouseGrid, ((Vector2)_foundationBlock.transform.position).ToIVec(), false);
    }


    public bool CanPlaceBlockAt(Block block, IVector2 pos)
    {
        for (int i = 0; i < block.size.X; i++)
        {
            for (int j = 0; j < block.size.Y; j++)
            {
                IVector2 check = new IVector2(i, j) + pos;
                if (!Game.InBounds(check) || this[check] != null) return false;
            }
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
      //var bb = new BBox(new(-10, -10), new(20, 40));
      //var from = transform.position + new Vector3(bb.from.X, bb.from.Y, 0.0f);
      //var to = transform.position + new Vector3(bb.to.X, bb.to.Y, 0.0f);
      //Gizmos.DrawLine(from, new Vector3(to.x, from.y, to.z));
      //Gizmos.DrawLine(new Vector3(to.x, from.y, to.z), to);
      //Gizmos.DrawLine(new Vector3(from.x, to.y, to.z), to);
      //Gizmos.DrawLine(from, new Vector3(from.x, to.y, to.z));
    }

    public void CreateFallingGridFromUnstable(){
        List<Block> all_unstable = Game.I.HouseGrid.AllUnstableBlocks();
        foreach (Block block in all_unstable)
        {
            block.RemoveFromGrid();
        }
        if (all_unstable.Count > 0)
        {
            FallingGrid g = Instantiate(Game.I.fallingGridPrefab).GetComponent<FallingGrid>();
            g.fallingBlocks = all_unstable;
        }
    }

    //private static void CopyData(BlockRecord[,] src, BlockRecord[,] dst, IVector2 dst_pos)
    //{
    //    for (int i = 0; i < src.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < src.GetLength(1); j++)
    //        {
    //            dst[i + dst_pos.X, j + dst_pos.Y] = dst[i, j];
    //        }
    //    }
    //}

    public List<Block> AllUnstableBlocks()
    {
        Search.ResetSearchFlags(_allBlocks);
        Search.RunAboveBFS(_foundationBlock);

        List<Block> all_unstable_blocks = new();
        foreach (Block b in _allBlocks)
        {
            if (!b.boxSearchData.visited)
            {
                all_unstable_blocks.Add(b);
            }
            else
            {
                for (int i = 0; i < b.BlocksBelow.Count; i++)
                {
                    if (!b.BlocksBelow[i].boxSearchData.visited)
                    {
                        b.RemoveLinkWithBelow(b.BlocksBelow[i]);
                        i--;
                    }
                }
            }
        }
        return all_unstable_blocks;
    }
}
