using UnityEngine;

/// <summary>
/// Holds two versions of a sprite, "natural" and "occluded"
/// </summary>
[System.Serializable]
public struct Sprite2
{
    public Sprite natural;
    public Sprite occluded;
}