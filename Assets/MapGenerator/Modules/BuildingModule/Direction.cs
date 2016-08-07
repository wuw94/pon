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
}