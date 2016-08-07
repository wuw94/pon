using UnityEngine;
using UnityEngine.Networking;

public class BaseModule : Module
{
    [SyncVar]
    public Vector2 SpawnA;
    [SyncVar]
    public Vector2 SpawnB;

    public Sprite floor;

    public override void Initialize()
    {
        AddBase();
    }

    private void AddBase()
    {
        SpawnA = new Vector2(0, (int)(map.dimension.y / 2));
        map.usage_chart.Use(SpawnA);


        SpawnB = new Vector2((int)(map.dimension.x - 1), (int)(map.dimension.y / 2));
        map.usage_chart.Use(SpawnB);
    }

    public override void Draw()
    {
        MapGenerator.AddToTexture(ref texture, SpawnA, floor.texture);
        MapGenerator.AddToTexture(ref texture, SpawnB, floor.texture);
        GetComponent<SpriteRenderer>().sprite = MapGenerator.ConvertToSprite(texture);
    }
}
