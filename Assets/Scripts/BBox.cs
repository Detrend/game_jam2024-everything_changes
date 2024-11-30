using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct BBox
{
    public IVector2 from;
    public IVector2 to;

    public readonly IVector2 Size => to - from;


    public readonly Vector2 Center => from.ToVec() + Size.ToVec() / 2;

    public BBox(IVector2 pos, IVector2 size)
    {
        from = pos;
        to = from + size;
    }

    public readonly BBox Extend(BBox b)
    {
        return new BBox(
            IVector2.Min(from, b.from),
            IVector2.Max(to, b.to)
        );
    }

    public readonly bool Contains(BBox b) => from <= b.from && to >= b.to;

    public readonly bool Contains(IVector2 a) => a.X >= from.X && a.X < to.X && a.Y >= from.Y && a.Y < to.Y;

    public static bool operator ==(BBox left, BBox right)
    {
        return left.from == right.from &&
               left.to == right.to;
    }
    public static bool operator !=(BBox left, BBox right) => !(left == right);

    public readonly override bool Equals(object obj)
    {
        if (obj is BBox other)
        {
            return this == other; // Use the == operator
        }
        return false;
    }

    public readonly override int GetHashCode() => HashCode.Combine(from.GetHashCode(), to.GetHashCode());


    public readonly IEnumerable<IVector2> AllCoordinates
    {
        get
        {
            foreach (IVector2 x in Size.AllCoordinates) yield return x + from;
        }
    }

    public override string ToString() => "BBox(From=" + from.ToString() + ", To=" + to.ToString() + ")";
}
