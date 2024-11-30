using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;



[Serializable]
public struct IVector2
{
    public int X, Y;
    public IVector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static IVector2 Zero => new (0, 0);

    public static IVector2 operator +(IVector2 a, IVector2 b) => new(a.X + b.X, a.Y + b.Y);

    public static IVector2 operator -(IVector2 a, IVector2 b) => new(a.X - b.X, a.Y - b.Y);

    public static bool operator >(IVector2 a, IVector2 b) => a.X > b.X && a.Y > b.Y;
    public static bool operator <(IVector2 a, IVector2 b) => a.X < b.X && a.Y < b.Y;
    public static bool operator>=(IVector2 a, IVector2 b) => a.X >= b.X && a.Y >= b.Y;
    public static bool operator<=(IVector2 a, IVector2 b) => a.X <= b.X && a.Y <= b.Y;

    public static IVector2 Min(IVector2 a, IVector2 b) => new(Mathf.Min(a.X, b.X), Mathf.Min(a.Y, b.Y));
    public static IVector2 Max(IVector2 a, IVector2 b) => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));


    public IEnumerable<IVector2> AllCoordinates
    {
        get
        {
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    yield return new IVector2 (i, j);
                }
            }
        }
    }


    public static bool operator ==(IVector2 left, IVector2 right)
    {
        return left.X == right.X &&
               left.Y == right.Y;
    }
    public static bool operator !=(IVector2 left, IVector2 right) => !(left == right);

    public override bool Equals(object obj)
    {
        if (obj is IVector2 other)
        {
            return this == other; // Use the == operator
        }
        return false;
    }

    public readonly override int GetHashCode() => HashCode.Combine(X, Y);

    public readonly Vector2 ToVec() => new(X, Y);


    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ")";
    }
}



[Serializable]
public readonly struct BBox
{
    private readonly IVector2 _from;
    private readonly IVector2 _to;

    public readonly IVector2 From => _from;
    public readonly IVector2 To => _to;

    public readonly IVector2 Size => To - From;

    public BBox(IVector2 pos, IVector2 size)
    {
        _from = pos;
        _to = _from + size;
    }

    public readonly BBox Extend(BBox b)
    {
        return new BBox(
            IVector2.Min(From, b.From),
            IVector2.Max(To, b.To)
        );
    }

    public bool Contains(BBox b) => From <= b.From && To >= b.To;

    public bool Contains(IVector2 a) => a.X >= From.X && a.X < To.X && a.Y >= From.Y && a.Y < To.Y;

    public static bool operator ==(BBox left, BBox right)
    {
        return left.From == right.From &&
               left.To == right.To;
    }
    public static bool operator !=(BBox left, BBox right) => !(left == right);

    public override bool Equals(object obj)
    {
        if (obj is BBox other)
        {
            return this == other; // Use the == operator
        }
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(From.GetHashCode(), To.GetHashCode());


    public IEnumerable<IVector2> AllCoordinates
    {
        get
        {
            foreach (IVector2 x in Size.AllCoordinates) yield return x + From;
        }
    }
}


public static class Vec2Ext
{
    public static IVector2 ToIVec(this Vector2 v) => new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
}



public class Block : MonoBehaviour
{
    BBox _BBox;

    public BBox BBox => _BBox;

    public IVector2 size;


    bool grabbed;
    IVector2 grabbedPart;
    IVector2 lastValidPosition;

    bool outsideOfGrid = true;


    SpriteRenderer _spriteRenderer;
    Material _material;


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
    }

    public void Place(IVector2 pos)
    {
        _BBox = new BBox(pos, size);
    }


    public void Grab()
    {
        if (!outsideOfGrid)
            Game.I.HouseGrid.RemoveBlockAt(BBox.From);

        grabbed = true;
        grabbedPart = Game.MouseWorldPos.ToIVec() - _BBox.From;
        lastValidPosition = BBox.From;
        _material.SetInt("_Ghost", 1);
        _spriteRenderer.sortingOrder = 100;
    }

    private void Update()
    {
        if (grabbed)
        {
            IVector2 int_pos = Game.MouseWorldPos.ToIVec() - grabbedPart;
            transform.position = int_pos.ToVec();

            bool valid_placement = Game.I.HouseGrid.BlockFits(this, int_pos);
            _material.SetInt("_PlacementValid", valid_placement ? 1 : 0);

            if (valid_placement) lastValidPosition = int_pos;
            if (Input.GetMouseButtonUp(0))
            {
                transform.position = lastValidPosition.ToVec();
                Game.I.HouseGrid.AddBlock(this, lastValidPosition);
                outsideOfGrid = false;
                grabbed = false;
                _material.SetInt("_Ghost", 0);
                _spriteRenderer.sortingOrder = 0;
            }
        }
    }
}
