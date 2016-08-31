using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GrassModule : Module
{
    public Sprite2 grass;

    public override void Initialize()
    {
        return;
    }

    public override void Draw()
    {
        for (int x = 0; x < map.dimension.x; x++)
            for (int y = 0; y < map.dimension.y; y++)
                MapGenerator.AddToTexture(ref texture, new Vector2(x, y), grass);
    }
}