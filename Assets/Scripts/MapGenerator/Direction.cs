[System.Serializable]
public enum Direction
{
    North, East, South, West
}

public static class DirectionExtension
{
    public static Direction Opposite(this Direction dir)
    {
        return (Direction)(((int)dir + 2) % 4);
    }

    public static Point Offset(this Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return Point.up;
            case Direction.East:
                return Point.right;
            case Direction.South:
                return Point.down;
            case Direction.West:
                return Point.left;
        }
        return Point.zero;
    }

    /// <summary>
    /// Move clockwise and get the next direction.
    /// </summary>
    /// <returns></returns>
    public static Direction Next(this Direction dir)
    {
        return (Direction)(((int)dir + 1) % 4);
    }

    /// <summary>
    /// Move counterclockwise and get the next direction.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Direction Previous(this Direction dir)
    {
        return (Direction)(((int)dir + 3) % 4);
    }
}