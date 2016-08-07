public struct IntRange
{
    public int min;
    public int max;

    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}

public static class IntRangeExtension
{
    public static int Random(this IntRange mine)
    {
        return UnityEngine.Random.Range(mine.min, mine.max);
    }
}