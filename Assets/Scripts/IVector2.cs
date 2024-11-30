using System;
using System.Collections.Generic;
using UnityEngine;



public static class Math2
{
    public static int Square(int x) => x * x;
    public static float Square(float x) => x * x;

}

[Serializable]
public struct IVector2
{
    public int X, Y;
    public IVector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static IVector2 Zero => new(0, 0);

    public static IVector2 operator +(IVector2 a, IVector2 b) => new(a.X + b.X, a.Y + b.Y);

    public static IVector2 operator -(IVector2 a, IVector2 b) => new(a.X - b.X, a.Y - b.Y);

    public static bool operator >(IVector2 a, IVector2 b) => a.X > b.X && a.Y > b.Y;
    public static bool operator <(IVector2 a, IVector2 b) => a.X < b.X && a.Y < b.Y;
    public static bool operator >=(IVector2 a, IVector2 b) => a.X >= b.X && a.Y >= b.Y;
    public static bool operator <=(IVector2 a, IVector2 b) => a.X <= b.X && a.Y <= b.Y;

    public static IVector2 Min(IVector2 a, IVector2 b) => new(Mathf.Min(a.X, b.X), Mathf.Min(a.Y, b.Y));
    public static IVector2 Max(IVector2 a, IVector2 b) => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));

    public static int DistanceL1(IVector2 a, IVector2 b) => Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    public static float DistanceL2(IVector2 a, Vector2 b) => Mathf.Sqrt(Math2.Square(a.X - b.x) + Math2.Square(a.Y - b.y));

    public IEnumerable<IVector2> AllCoordinates
    {
        get
        {
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    yield return new IVector2(i, j);
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



public static class Vec2Ext
{
    public static IVector2 ToIVec(this Vector2 v) => new(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

    public static IVector2 ToIVec(this Vector3 v) => ((Vector2) v).ToIVec();

    public static Vector2 ToVec2(this Vector3 v) => new(v.x, v.y);
}