using UnityEngine;

[System.Serializable]
public class Texture2D2
{
    public Texture2D natural;
    public Texture2D occluded;

    public Texture2D2(Texture2D natural, Texture2D occluded)
    {
        this.natural = natural;
        this.occluded = occluded;
    }
}
