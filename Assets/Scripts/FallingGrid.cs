using System.Collections.Generic;
using UnityEngine;

public class FallingGrid : BlockGrid
{
    public List<Block> fallingBlocks;

    float fallOffset = 0f;
    float fallSpeed = 0f;

    public float fallAcceleration = 1f;

    protected override void MyStart()
    {
        base.MyStart();
        foreach (Block block in fallingBlocks)
            block.Place(this, block.BBox.from, false);
        Game.I.FallingGrids.Add(this);
    }



    private void Update()
    {
        foreach (Block b in _allBlocks)
        {
            b.transform.position = b.BBox.from.ToVec() - new Vector2(0, fallOffset);
        }

        fallSpeed += fallAcceleration * Time.deltaTime;
        float new_fall_offset = fallOffset + fallSpeed * Time.deltaTime;
        if (Mathf.FloorToInt(new_fall_offset) != Mathf.FloorToInt(fallOffset))
        {
            //Check if any blocks have become stable...
            int current_move = Mathf.FloorToInt(new_fall_offset);

            bool stopped = false;
            foreach (IVector2 pos in BBox.AllCoordinates)
            {
                BlockRecord top_block = this[pos];
                if (top_block == null) continue;

                IVector2 real_pos = pos - new IVector2(0, current_move);
                BlockRecord bot_block = Game.I.HouseGrid[real_pos - new IVector2(0, 1)];

                if (top_block != null && bot_block != null)
                {
                    top_block.block.LinkWithBelow(bot_block.block);
                    stopped = true;
                }
            }
            if (stopped)
            {
                Search.ResetSearchFlags(Game.I.HouseGrid.AllBlocks);
                Search.RunAboveBFS(Game.I.HouseGrid.FoundationBlock);

                bool some_blocks_left = false;
                // all marked true = stable. All marked false = unstable.
                for (int i = 0; i < _allBlocks.Count; i++)
                {
                    Block b = _allBlocks[i];
                    if (b.boxSearchData.visited)
                    {
                        b.RemoveFromGrid();
                        b.Place(Game.I.HouseGrid, b.BBox.from - new IVector2(0, current_move), true);
                        i--;
                        // TODO APPLY DAMAGE. BOX STOPPED
                    }
                    else
                    {
                        some_blocks_left = true;
                        //split ties with all above stopped blocks
                        foreach (Block b_above in b.BlocksAbove)
                        {
                            if (b_above.boxSearchData.visited) b.RemoveLinkWithAbove(b_above);
                        }
                    }
                }
                if (!some_blocks_left)
                {
                    Game.I.FallingGrids.Remove(this);
                    Destroy(this);
                }
            }
        }
        fallOffset = new_fall_offset;
        transform.position = new Vector3(0, -fallOffset, 0);
    }


    public bool CanPlaceBlockAt(Block block, IVector2 pos)
    {
        for (int i = 0; i < block.size.X; i++)
        {
            for (int j = Mathf.FloorToInt(fallOffset); j < block.size.Y + Mathf.CeilToInt(fallOffset); j++)
            {
                IVector2 check = new IVector2(i, j) + pos;
                if (!BBox.Contains(check) || this[check] != null) return false;
            }
        }
        return true;
    }
}
