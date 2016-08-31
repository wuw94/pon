using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Building : Container
{
    private const int MAX_ROOMS = 8;
    private const int ROOM_SIZE_GUARANTEES_OBJECTIVE = 5;
    private readonly IntRange ROOM_SIZE = new IntRange(2, 6);
    private const int CONTROL_POINT_SIZE = 6;
    private const float CHANCE_FOR_ENTRANCE_OUTSIDE = 0.5f;
    private const float CHANCE_FOR_ENTRANCE_OUTSIDE_EXTRA = 0.3f;
    private const int MIN_ENTRANCE_PER_ROOM = 2;
    private const float CHANCE_FOR_WINDOW_INSIDE = 0.4f;
    private const float CHANCE_FOR_WINDOW_OUTSIDE = 0.4f;

    public List<Room> rooms = new List<Room>();

    public Building()
        : base() { }
    
    public Building(Container owner, Point position, Point dimension)
        : base(owner, position, dimension)
    {
        SplitRooms();
        Interpret();
        MakeEntrances();
    }
    
    private void SplitRooms()
    {
        //List<Container> containers = new Container(this, new Point(), dimension).RecursiveDivision(room_size);

        int breakpoint_x_1 = (dimension.x - CONTROL_POINT_SIZE) / 2;
        int breakpoint_x_2 = dimension.x - breakpoint_x_1;
        int breakpoint_y_1 = (dimension.y - CONTROL_POINT_SIZE) / 2;
        int breakpoint_y_2 = dimension.y - breakpoint_y_1;

        Point p1 = new Point(0, 0);
        Point d1 = new Point(breakpoint_x_1, breakpoint_y_2);
        Point p2 = new Point(breakpoint_x_1, 0);
        Point d2 = new Point(dimension.x - breakpoint_x_1, breakpoint_y_1);
        Point p3 = new Point(0, breakpoint_y_2);
        Point d3 = new Point(breakpoint_x_2, dimension.y - breakpoint_y_2);
        Point p4 = new Point(breakpoint_x_2, breakpoint_y_1);
        Point d4 = new Point(dimension.x - breakpoint_x_2, dimension.y - breakpoint_y_1);


        
        foreach (Container container in new Container(this, p1, d1).RecursiveDivision(ROOM_SIZE))
            rooms.Add(new Room(this, container));
        foreach (Container container in new Container(this, p2, d2).RecursiveDivision(ROOM_SIZE))
            rooms.Add(new Room(this, container));
        foreach (Container container in new Container(this, p3, d3).RecursiveDivision(ROOM_SIZE))
            rooms.Add(new Room(this, container));
        foreach (Container container in new Container(this, p4, d4).RecursiveDivision(ROOM_SIZE))
            rooms.Add(new Room(this, container));

        // there cannot be more than a certain number of rooms
        while (rooms.Count > MAX_ROOMS)
            rooms.RemoveAt(UnityEngine.Random.Range(0,rooms.Count));
    }

    private void Interpret()
    {
        foreach (Room room in rooms)
            room.Interpret(rooms);
    }

    private void MakeEntrances()
    {
        HashSet<Room> visited = new HashSet<Room>();
        
        foreach (Room room1 in rooms)
        {
            if (!visited.Contains(room1))
            {
                foreach (Room room2 in room1.adjacent_rooms)
                {
                    if (room1.MakeEntranceAgainstOtherRoom(room2))
                    {
                        visited.Add(room2);
                    }
                    if (Chance.Try(CHANCE_FOR_WINDOW_INSIDE))
                        room1.MakeWindowAgainstOtherRoom(room2);
                }
            }
            if (Chance.Try(CHANCE_FOR_ENTRANCE_OUTSIDE))
                room1.MakeEntranceNotAgainstAnyRoom(rooms);
            if (Chance.Try(CHANCE_FOR_ENTRANCE_OUTSIDE_EXTRA))
                room1.MakeEntranceNotAgainstAnyRoom(rooms);
            if (Chance.Try(CHANCE_FOR_WINDOW_OUTSIDE))
                room1.MakeWindowNotAgainstAnyRoom(rooms);
            while (room1.entrances.Count < MIN_ENTRANCE_PER_ROOM)
                room1.MakeEntranceNotAgainstAnyRoom(rooms);
            visited.Add(room1);
        }
    }

    public void Draw(ref Texture2D2 texture)
    {
        DrawFloors(ref texture);
        DrawWalls(ref texture);
        DrawWindows(ref texture);
        DrawEntrances(ref texture);
    }

    private void DrawFloors(ref Texture2D2 texture)
    {
        BuildingModule module = GameObject.FindObjectOfType<BuildingModule>();
        for (int x = 0; x < this.dimension.x; x++)
            for (int y = 0; y < this.dimension.y; y++)
                MapGenerator.AddToTexture(ref texture, this.Position(Depth.World) + new Point(x, y), module.building_floor);
    }

    private void DrawWalls(ref Texture2D2 texture)
    {
        List<DirectionalPair> all_walls = new List<DirectionalPair>();
        foreach (Room room in rooms)
            foreach (Directional wall in room.walls)
                if (!all_walls.Any<DirectionalPair>(w => w == new DirectionalPair(wall, wall.OtherSide())))
                    all_walls.Add(new DirectionalPair(wall, wall.OtherSide()));

        foreach (DirectionalPair wall_pair in all_walls)
        {
            DrawWall(wall_pair.first, ref texture);
            DrawWall(wall_pair.second, ref texture);
        }
    }

    private void DrawWall(Directional wall, ref Texture2D2 texture)
    {
        BuildingModule module = GameObject.FindObjectOfType<BuildingModule>();
        if (wall.direction == Direction.North)
            MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_north);
        else if (wall.direction == Direction.East)
            MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_east);
        else if (wall.direction == Direction.South)
            MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_south);
        else
            MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_west);
    }

    private void DrawWindows(ref Texture2D2 texture)
    {
        List<DirectionalPair> all_windows = new List<DirectionalPair>();
        foreach (Room room in rooms)
            foreach (Directional window in room.windows)
                if (!all_windows.Any<DirectionalPair>(w => w == new DirectionalPair(window, window.OtherSide())))
                    all_windows.Add(new DirectionalPair(window, window.OtherSide()));

        foreach (DirectionalPair window_pair in all_windows)
        {
            DrawWindow(window_pair.first, ref texture);
            DrawWindow(window_pair.second, ref texture);
        }
    }

    private void DrawWindow(Directional window, ref Texture2D2 texture)
    {
        BuildingModule module = GameObject.FindObjectOfType<BuildingModule>();
        if (window.direction == Direction.North)
            MapGenerator.AddToTexture(ref texture, window.Position(Depth.World), module.window_north);
        else if (window.direction == Direction.East)
            MapGenerator.AddToTexture(ref texture, window.Position(Depth.World), module.window_east);
        else if (window.direction == Direction.South)
            MapGenerator.AddToTexture(ref texture, window.Position(Depth.World), module.window_south);
        else
            MapGenerator.AddToTexture(ref texture, window.Position(Depth.World), module.window_west);
    }

    private void DrawEntrances(ref Texture2D2 texture)
    {
        List<DirectionalPair> all_entrances = new List<DirectionalPair>();
        foreach (Room room in rooms)
            foreach (Directional entrance in room.entrances)
                if (!all_entrances.Any<DirectionalPair>(w => w == new DirectionalPair(entrance, entrance.OtherSide())))
                    all_entrances.Add(new DirectionalPair(entrance, entrance.OtherSide()));

        foreach (DirectionalPair entrance_pair in all_entrances)
        {
            DrawEntrance(entrance_pair.first, ref texture);
            DrawEntrance(entrance_pair.second, ref texture);
        }
    }

    private void DrawEntrance(Directional entrance, ref Texture2D2 texture)
    {
        BuildingModule module = GameObject.FindObjectOfType<BuildingModule>();
        if (entrance.direction == Direction.North)
            MapGenerator.AddToTexture(ref texture, entrance.Position(Depth.World), module.entrance_north);
        else if (entrance.direction == Direction.East)
            MapGenerator.AddToTexture(ref texture, entrance.Position(Depth.World), module.entrance_east);
        else if (entrance.direction == Direction.South)
            MapGenerator.AddToTexture(ref texture, entrance.Position(Depth.World), module.entrance_south);
        else
            MapGenerator.AddToTexture(ref texture, entrance.Position(Depth.World), module.entrance_west);
    }

    public override string ToString()
    {
        string to_return = "Building - Pos" + this.position_at_current_depth + " Dim" + this.dimension + " Depth(" + this.depth + ")";
        to_return += "\n Depth at World: Pos" + this.Position(Depth.World);

        return to_return;
    }
}
