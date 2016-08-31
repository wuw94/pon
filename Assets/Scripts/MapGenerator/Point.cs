using UnityEngine;
using UnityEngine.Networking;



/// <summary>
/// Points are like Vector2's, but can only take in ints.
/// </summary>
[System.Serializable]
public struct Point
{
    public static Point zero = new Point(0, 0);
    public static Point left = new Point(-1, 0);
    public static Point right = new Point(1, 0);
    public static Point up = new Point(0, 1);
    public static Point down = new Point(0, -1);

    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + this.x + "," + this.y + ")";
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return p1.x == p2.x && p1.y == p2.y;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return p1.x != p2.x || p1.y != p2.y;
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.x + p2.x, p1.y + p2.y);
    }

    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.x - p2.x, p1.y - p2.y);
    }
    
    public static implicit operator UnityEngine.Vector2(Point p)
    {
        return new UnityEngine.Vector2(p.x, p.y);
    }

    public static implicit operator Point(UnityEngine.Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);
    }
}