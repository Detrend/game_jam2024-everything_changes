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

        CheckHittingAnotherGrid(Game.I.HouseGrid);
        foreach (FallingGrid g in Game.I.FallingGrids)
        {
            CheckHittingAnotherGrid(g);
        }

        fallOffset = new_fall_offset;
        transform.position = new Vector3(0, -fallOffset, 0);
    }


    private void CheckHittingAnotherGrid(BlockGrid another)
    {
        FallingGrid another_fg = another as FallingGrid;
        float another_offset = (another_fg == null) ? 0f : another_fg.fallOffset;
        float another_speed = (another_fg == null) ? 0f : another_fg.fallSpeed;


        float current_offset = fallOffset - another_offset;
        float next_offset = current_offset + (fallSpeed - another_speed) * Time.deltaTime;

        if (Mathf.FloorToInt(current_offset) != Mathf.FloorToInt(next_offset))
        {
            //Check if any blocks have become stable...
            int current_move = Mathf.FloorToInt(next_offset);

            bool grids_overlap = false;
            foreach (IVector2 pos in Game.I.gameRegion.AllCoordinates)
            {
                BlockRecord top_block = this[pos];
                if (top_block == null) continue;

                IVector2 real_pos = pos - new IVector2(0, current_move);
                BlockRecord bot_block = another[real_pos - new IVector2(0, 1)];

                if (top_block != null && bot_block != null)
                {
                    top_block.block.LinkWithBelow(bot_block.block);
                    grids_overlap = true;
                }
            }
            if (grids_overlap)
            {
                Search.ResetSearchFlags(another.AllBlocks);
                Search.ResetSearchFlags(AllBlocks);
                foreach (Block b in another.AllBlocks)
                    Search.RunAboveBFS(b);

                int total_boxes_present = FilledBlockAmount;

                // all marked true = stable. All marked false = unstable.
                for (int i = 0; i < _allBlocks.Count; i++)
                {
                    Block b = _allBlocks[i];
                    if (b.boxSearchData.visited)
                    {
                        b.RemoveFromGrid();
                        b.Place(another, b.BBox.from - new IVector2(0, current_move), true);
                        i--;
                        // TODO APPLY DAMAGE. BOX STOPPED
                    }
                    else
                    {
                        //split ties with all above stopped blocks
                        for (int j = 0; j < b.BlocksAbove.Count; j++)
                        {
                            if (b.BlocksAbove[j].boxSearchData.visited)
                            {
                                b.RemoveLinkWithAbove(b.BlocksAbove[j]);
                                j--;
                            }
                        }
                    }
                }

                if (another_fg != null)
                {
                    int boxes_transferred = total_boxes_present - FilledBlockAmount;
                    another_fg.fallSpeed = Mathf.Lerp(another_fg.fallSpeed, fallSpeed, 1f * boxes_transferred / another_fg.FilledBlockAmount);
                }
            }
        }
    }


    public bool CanPlaceBlockAt(Block block, IVector2 pos)
    {
        for (int i = 0; i < block.size.X; i++)
        {
            for (int j = Mathf.FloorToInt(fallOffset); j < block.size.Y + Mathf.CeilToInt(fallOffset); j++)
            {
                IVector2 check = new IVector2(i, j) + pos;
                if (Game.InBounds(check) && this[check] != null) return false;
            }
        }
        return true;
    }

    private void OnDestroy()
    {
        Game.I.FallingGrids.Remove(this);
    }
}
