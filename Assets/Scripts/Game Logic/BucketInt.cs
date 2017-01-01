using UnityEngine;

[System.Serializable]
public class BucketInt
{
    public int max;
    [UnityEngine.HideInInspector]
    public int current;

    public BucketInt(int max)
    {
        this.max = max;
        this.current = max;
    }
}

public static class BucketExtension
{
    public static bool IsFull(this BucketInt mine)
    {
        return mine.current == mine.max;
    }

    public static bool IsEmpty(this BucketInt mine)
    {
        return mine.current == 0;
    }

    public static void Refill(this BucketInt mine)
    {
        mine.current = mine.max;
    }
}