using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System;

public class SyncListPoint : SyncListStruct<Point> { }
public class SyncListVector2 : SyncListStruct<Vector2> { }

public class SyncListPointDirection : UnityEngine.Networking.SyncListStruct<PointDirection> { }

public class BuildingModule : Module
{
    private const float WALL_THICKNESS = 0.05f;
    private const int BUILDING_PAD = 2;
    public IntRange building_size = new IntRange(4, 5);

    public Container world;
    public List<Building> buildings = new List<Building>();
    public GameObject collider_wall;
    public GameObject collider_window;

    public GameObject nucleus;

    public GameObject room_light;
    public GameObject building_light;

    public Sprite2 building_floor;
    public Sprite2 debug_floor;
    
    public Sprite2 wall_north;
    public Sprite2 wall_east;
    public Sprite2 wall_south;
    public Sprite2 wall_west;

    public Sprite2 window_north;
    public Sprite2 window_east;
    public Sprite2 window_south;
    public Sprite2 window_west;

    public Sprite2 entrance_north;
    public Sprite2 entrance_east;
    public Sprite2 entrance_south;
    public Sprite2 entrance_west;

    // Synced variables
    private SyncListPoint _sync_floors = new SyncListPoint();
    private SyncListPointDirection _sync_entrances = new SyncListPointDirection();
    private SyncListPointDirection _sync_windows = new SyncListPointDirection();
    private SyncListPointDirection _sync_walls = new SyncListPointDirection();
    

    public override void Initialize()
    {
        world = new Container(null, Point.zero, map.dimension);
        MakeBuildings();
        MakeLights();
        MakeNucleus();
        SyncVariables();
    }

    private void MakeBuildings()
    {
        /*
        for (int i = 0; i < 20; i++)
        {
            if (buildings.Count >= 3)
                break;
            Point rand_dimension = new Point(building_size.Random(), building_size.Random());
            Point rand_position = new Point(UnityEngine.Random.Range(0, map.dimension.x - rand_dimension.x), UnityEngine.Random.Range(0, map.dimension.y - rand_dimension.y));
            if (map.usage_chart.Count(rand_position, rand_dimension, UsageInfo.Used) == 0)
                MakeBuilding(rand_position, rand_dimension);
        }
        */
        
        MakeBuilding(new Point(8,3), new Point(14,14));
    }

    private void MakeBuilding(Point position, Point dimension)
    {
        buildings.Add(new Building(world, position, dimension));
        map.usage_chart.Use(position - new Point(BUILDING_PAD, BUILDING_PAD), dimension + new Point(BUILDING_PAD * 2, BUILDING_PAD * 2));
    }
    
    private void SyncVariables()
    {
        foreach (Building b in buildings)
        {
            for (int x = 0; x < b.dimension.x; x++)
                for (int y = 0; y < b.dimension.y; y++)
                    _sync_floors.Add(b.Position(Depth.World) + new Point(x, y));
            foreach (Room r in b.rooms)
            {
                foreach (Directional d in r.entrances)
                    _sync_entrances.Add(new PointDirection(d.Position(Depth.World), d.direction));
                foreach (Directional d in r.windows)
                    _sync_windows.Add(new PointDirection(d.Position(Depth.World), d.direction));
                foreach (Directional d in r.walls)
                    _sync_walls.Add(new PointDirection(d.Position(Depth.World), d.direction));
            }
        }
    }

    private void MakeColliderWall()
    {
        List<PointDirection> all_walls = new List<PointDirection>();
        foreach (PointDirection pd in _sync_walls)
            all_walls.Add(pd);
        List<PointDirection> to_add = new List<PointDirection>();
        foreach (PointDirection pd in all_walls)
            if (!all_walls.Any(w => w == pd.OtherSide()))
                to_add.Add(pd.OtherSide());
        foreach (PointDirection pd in to_add)
            all_walls.Add(pd);

        while (all_walls.Count > 0)
        {
            this.collider_wall.AddComponent<PolygonCollider2D>().SetPath(0, GetPath(ref all_walls));
            //polygon_collider.SetPath(polygon_collider.pathCount++, GetPath(ref all_walls));
        }
    }

    private void MakeColliderWindow()
    {
        List<PointDirection> all_windows = new List<PointDirection>();
        foreach (PointDirection pd in _sync_windows)
            all_windows.Add(pd);
        List<PointDirection> to_add = new List<PointDirection>();
        foreach (PointDirection pd in all_windows)
            if (!all_windows.Any(w => w == pd.OtherSide()))
                to_add.Add(pd.OtherSide());
        foreach (PointDirection pd in to_add)
            all_windows.Add(pd);

        while (all_windows.Count > 0)
        {
            this.collider_window.AddComponent<PolygonCollider2D>().SetPath(0, GetPath(ref all_windows));
        }
    }

    private Vector2[] GetPath(ref List<PointDirection> all_walls)
    {
        List<Vector2> to_return = new List<Vector2>();

        List<PointDirection> to_remove = new List<PointDirection>();
        PointDirection first_wall = all_walls[0];
        PointDirection current_wall = new PointDirection(first_wall.point, first_wall.direction);
        to_return.Add(Beginning(current_wall, true));

        while (true)
        {
            PointDirection[] next_candidate = new PointDirection[4];
            next_candidate[0] = new PointDirection(current_wall.point, current_wall.direction.Previous());
            next_candidate[1] = new PointDirection(current_wall.point + current_wall.direction.Previous().Offset(), current_wall.direction);
            next_candidate[2] = new PointDirection(current_wall.point + current_wall.direction.Previous().Offset() + current_wall.direction.Offset(), current_wall.direction.Next());
            next_candidate[3] = new PointDirection(current_wall.point + current_wall.direction.Offset(), current_wall.direction.Opposite());

            /*
            // against self
            if (all_walls.Any(w => w == next_candidate[0]))
            {
                to_return.Add(End(current_wall, true));
                current_wall = all_walls.Find(w => w == next_candidate[0]);
            }
            */
            // straight
            if (all_walls.Any(w => w == next_candidate[1]))
            {
                current_wall = all_walls.Find(w => w == next_candidate[1]);
            }
            /*
            // away from self
            else if (all_walls.Any(w => w == next_candidate[2]))
            {
                to_return.Add(AwayFrom(current_wall));
                current_wall = all_walls.Find(w => w == next_candidate[2]);
            }
            */
            // completely reverse
            else
            {
                to_return.Add(End(current_wall, false));
                current_wall = all_walls.Find(w => w == next_candidate[3]);
                to_return.Add(Beginning(current_wall, false));
            }
            to_remove.Add(current_wall);
            if (current_wall == first_wall)
            {
                break;
            }
        }
        foreach (PointDirection pd in to_remove)
            all_walls.Remove(pd);
        return to_return.ToArray();
    }
    
    public Vector2 Beginning(PointDirection pd, bool use_pad)
    {
        if (pd.direction == Direction.North)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(1 - (use_pad ? WALL_THICKNESS : 0), 1 - WALL_THICKNESS);
        else if (pd.direction == Direction.East)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(1 - WALL_THICKNESS, (use_pad ? WALL_THICKNESS : 0));
        else if (pd.direction == Direction.South)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2((use_pad ? WALL_THICKNESS : 0), WALL_THICKNESS);
        else
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(WALL_THICKNESS, 1 - (use_pad ? WALL_THICKNESS : 0));
    }
    
    public Vector2 End(PointDirection pd, bool use_pad)
    {
        if (pd.direction == Direction.North)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2((use_pad ? WALL_THICKNESS : 0), 1 - WALL_THICKNESS);
        else if (pd.direction == Direction.East)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(1 - WALL_THICKNESS, 1 - (use_pad ? WALL_THICKNESS : 0));
        else if (pd.direction == Direction.South)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(1 - (use_pad ? WALL_THICKNESS : 0), WALL_THICKNESS);
        else
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(WALL_THICKNESS, (use_pad ? WALL_THICKNESS : 0));
    }
    
    public Vector2 AwayFrom(PointDirection pd)
    {
        if (pd.direction == Direction.North)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(-WALL_THICKNESS, 1 - WALL_THICKNESS);
        else if (pd.direction == Direction.East)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(1 - WALL_THICKNESS, 1 + WALL_THICKNESS);
        else if (pd.direction == Direction.South)
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(1 + WALL_THICKNESS, WALL_THICKNESS);
        else
            return new Vector2(MapGenerator.DRAW_PAD, MapGenerator.DRAW_PAD) + (Vector2)pd.point + new Vector2(WALL_THICKNESS, -WALL_THICKNESS);
    }

    public override void Draw()
    {
        DrawFloors();
        DrawEntrances();
        DrawWindows();
        DrawWalls();
        

        MakeColliderWall();
        MakeColliderWindow();
    }

    private void DrawFloors()
    {
        foreach (Point p in _sync_floors)
            MapGenerator.AddToTexture(ref texture, p, building_floor);
    }

    private void DrawEntrances()
    {
        List<PointDirection> all_entrances = new List<PointDirection>();
        foreach (PointDirection pd in _sync_entrances)
            all_entrances.Add(pd);
        foreach (PointDirection pd in _sync_entrances)
            if (!all_entrances.Any(w => w == pd.OtherSide()))
                all_entrances.Add(pd.OtherSide());

        foreach (PointDirection pd in all_entrances)
        {
            if (pd.direction == Direction.North)
                MapGenerator.AddToTexture(ref texture, pd.point, entrance_north);
            else if (pd.direction == Direction.East)
                MapGenerator.AddToTexture(ref texture, pd.point, entrance_east);
            else if (pd.direction == Direction.South)
                MapGenerator.AddToTexture(ref texture, pd.point, entrance_south);
            else
                MapGenerator.AddToTexture(ref texture, pd.point, entrance_west);
        }
    }

    private void DrawWindows()
    {
        List<PointDirection> all_windows = new List<PointDirection>();
        foreach (PointDirection pd in _sync_windows)
            all_windows.Add(pd);
        foreach (PointDirection pd in _sync_windows)
            if (!all_windows.Any(w => w == pd.OtherSide()))
                all_windows.Add(pd.OtherSide());

        foreach (PointDirection pd in all_windows)
        {
            if (pd.direction == Direction.North)
                MapGenerator.AddToTexture(ref texture, pd.point, window_north);
            else if (pd.direction == Direction.East)
                MapGenerator.AddToTexture(ref texture, pd.point, window_east);
            else if (pd.direction == Direction.South)
                MapGenerator.AddToTexture(ref texture, pd.point, window_south);
            else
                MapGenerator.AddToTexture(ref texture, pd.point, window_west);
        }
    }

    private void DrawWalls()
    {
        List<PointDirection> all_walls = new List<PointDirection>();
        foreach (PointDirection pd in _sync_walls)
            all_walls.Add(pd);
        foreach (PointDirection pd in _sync_walls)
            if (!all_walls.Any(w => w == pd.OtherSide()))
                all_walls.Add(pd.OtherSide());

        foreach (PointDirection pd in all_walls)
        {
            if (pd.direction == Direction.North)
                MapGenerator.AddToTexture(ref texture, pd.point, wall_north);
            else if (pd.direction == Direction.East)
                MapGenerator.AddToTexture(ref texture, pd.point, wall_east);
            else if (pd.direction == Direction.South)
                MapGenerator.AddToTexture(ref texture, pd.point, wall_south);
            else
                MapGenerator.AddToTexture(ref texture, pd.point, wall_west);
        }
    }

    private void MakeLights()
    {
        foreach (Building b in buildings)
        {
            // make light for building
            GameObject go = Instantiate<GameObject>(building_light);
            go.transform.position = new Vector3(    (b.Left(Depth.World) + b.Right(Depth.World)) / 2.0f + MapGenerator.DRAW_PAD,
                                                    (b.Top(Depth.World) + b.Bottom(Depth.World)) / 2.0f + MapGenerator.DRAW_PAD,
                                                    go.transform.position.z);
            NetworkServer.Spawn(go);

            /*
            foreach (Room r in b.rooms)
            {
                go = Instantiate<GameObject>(room_light);
                go.transform.position = new Vector3((r.Left(Depth.World) + r.Right(Depth.World)) / 2.0f + MapGenerator.DRAW_PAD,
                                                    (r.Top(Depth.World) + r.Bottom(Depth.World)) / 2.0f + MapGenerator.DRAW_PAD,
                                                    go.transform.position.z);
                NetworkServer.Spawn(go);
            }
            */
        }
    }

    private void MakeNucleus()
    {
        foreach (Building b in buildings)
        {
            GameObject go = Instantiate<GameObject>(nucleus);
            go.transform.position = new Vector3(    (b.Left(Depth.World) + b.Right(Depth.World)) / 2.0f + MapGenerator.DRAW_PAD - 3,
                                                    (b.Top(Depth.World) + b.Bottom(Depth.World)) / 2.0f + MapGenerator.DRAW_PAD - 3,
                                                    go.transform.position.z);
            NetworkServer.Spawn(go);
        }
    }
    
    public override void Reset()
    {
        foreach (Nucleus n in FindObjectsOfType<Nucleus>())
            Destroy(n.gameObject);
        MakeNucleus();
    }
}