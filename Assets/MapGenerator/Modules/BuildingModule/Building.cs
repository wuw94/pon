using UnityEngine;
using System.Collections.Generic;

public class Building : Container
{
    private BuildingModule module;
    
    public List<Room> rooms = new List<Room>();

    private IntRange room_size = new IntRange(3, 6);

    public Building()
        : base() { }
    
    public Building(BuildingModule module, Container owner, Point position, Point dimension)
        : base(owner, position, dimension)
    {
        this.module = module;
        SplitRooms();
        MakeEntrances();
        Debug.Log(this);
    }
    
    private void SplitRooms()
    {
        List<Container> containers = new Container(this, new Point(), dimension).RecursiveDivision(room_size);
        foreach (Container container in containers)
            rooms.Add(new Room(this, container));

    }

    private void MakeEntrances()
    {
        HashSet<Room> visited = new HashSet<Room>();
        
        foreach (Room room1 in rooms)
        {
            if (!visited.Contains(room1))
                foreach (Room room2 in rooms)
                    if (room1.MakeEntranceAgainstOtherRoom(room2))
                        visited.Add(room2);
            room1.MakeEntranceNotAgainstAnyRoom(rooms);
            visited.Add(room1);
        }
    }

    public void Draw(ref Texture2D texture)
    {
        DrawFloors(ref texture);
        DrawWalls(ref texture);
    }

    private void DrawFloors(ref Texture2D texture)
    {
        for (int x = 0; x < this.dimension.x; x++)
            for (int y = 0; y < this.dimension.y; y++)
                MapGenerator.AddToTexture(ref texture, this.Position(Depth.World) + new Point(x, y), module.building_floor.texture);
    }

    private void DrawWalls(ref Texture2D texture)
    {
        List<Wall> all_walls = new List<Wall>();
        foreach (Room room in rooms)
            foreach (Wall wall in room.walls)
                all_walls.Add(wall);

        foreach (Wall wall in all_walls)
        {
            if (wall.direction == Direction.North)
                MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_north.texture);
            else if (wall.direction == Direction.East)
                MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_east.texture);
            else if (wall.direction == Direction.South)
                MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_south.texture);
            else
                MapGenerator.AddToTexture(ref texture, wall.Position(Depth.World), module.wall_west.texture);
        }
    }

    public override string ToString()
    {
        string to_return = "Building - Pos" + this.position_at_current_depth + " Dim" + this.dimension + " Depth(" + this.depth + ")";
        to_return += "\n Depth at World: Pos" + this.Position(Depth.World);

        return to_return;
    }
}
