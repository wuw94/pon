using UnityEngine;
public enum Corner
{
    TopRight, BottomRight, BottomLeft, TopLeft
}

public static class CornerExtension
{
    public static Corner GetRandomCorner()
    {
        int num = UnityEngine.Random.Range(0, 4);
        if (num == 0)
            return Corner.BottomLeft;
        else if (num == 1)
            return Corner.BottomRight;
        else if (num == 2)
            return Corner.TopLeft;
        return Corner.TopRight;
    }

    public static Corner Opposite(this Corner c)
    {
        return (Corner)(((int)c + 2) % 4);
    }

    public static Vector2 Offset(this Corner c)
    {
        switch (c)
        {
            case Corner.TopRight:
                return Vector2.up + Vector2.right;
            case Corner.BottomRight:
                return Vector2.down + Vector2.right;
            case Corner.BottomLeft:
                return Vector2.down + Vector2.left;
            case Corner.TopLeft:
                return Vector2.up + Vector2.left;
        }
        return Vector2.zero;
    }
}