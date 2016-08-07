using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class BuildingModule : Module
{
    public IntRange building_size = new IntRange(10, 15);
    public int building_pad;

    public Container world;

    public List<Building> buildings = new List<Building>();


    public Sprite building_floor;

    public Sprite debug_floor;

    public Sprite wall_north;
    public Sprite wall_east;
    public Sprite wall_south;
    public Sprite wall_west;



    public override void Initialize()
    {
        world = new Container(null, Point.zero, map.dimension);

        for (int i = 0; i < 20; i++)
        {
            if (buildings.Count >= 3)
                break;
            Point rand_dimension = new Point(building_size.Random(), building_size.Random());
            Point rand_position = new Point(UnityEngine.Random.Range(0, map.dimension.x - rand_dimension.x), UnityEngine.Random.Range(0, map.dimension.y - rand_dimension.y));
            if (map.usage_chart.Count(rand_position, rand_dimension, UsageInfo.Used) == 0)
                MakeBuilding(rand_position, rand_dimension);
        }
    }

    private void MakeBuilding(Point position, Point dimension)
    {
        buildings.Add(new Building(this, world, position, dimension));
        map.usage_chart.Use(position - new Point(building_pad, building_pad), dimension + new Point(building_pad * 2, building_pad * 2));
    }
    
    public override void Draw()
    {
        foreach (Building b in buildings)
            b.Draw(ref texture);
        GetComponent<SpriteRenderer>().sprite = MapGenerator.ConvertToSprite(texture);
    }


}