public static class Chance
{
    public static bool Try(float chance)
    {
        if (chance < 0 || chance > 1)
            throw new System.Exception();
        return UnityEngine.Random.Range(0, 1.0f) < chance;
    }
}
