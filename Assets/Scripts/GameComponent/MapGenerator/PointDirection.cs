

[System.Serializable]
public struct PointDirection
{
    public Point point;
    public Direction direction;
    public PointDirection(Point point, Direction direction)
    {
        this.point = point;
        this.direction = direction;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(PointDirection pd1, PointDirection pd2)
    {
        return pd1.point == pd2.point && pd1.direction == pd2.direction;
    }

    public static bool operator !=(PointDirection pd1, PointDirection pd2)
    {
        return pd1.point != pd2.point || pd1.direction != pd2.direction;
    }

}

public static class PointDirectionExtension
{
    public static PointDirection OtherSide(this PointDirection pd)
    {
        return new PointDirection(pd.point + pd.direction.Offset(), pd.direction.Opposite());
    }
}