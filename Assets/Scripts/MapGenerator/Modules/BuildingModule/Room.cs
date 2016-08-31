using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Room : Container
{
    public List<Container> floors = new List<Container>();
    public List<Directional> entrances = new List<Directional>();
    public List<Directional> windows = new List<Directional>();
    public List<Directional> walls = new List<Directional>();


    [System.NonSerialized] private Dictionary<Room, List<DirectionalPair>> adjacent_walls = new Dictionary<Room, List<DirectionalPair>>();
    [System.NonSerialized] private List<Directional> isolated_walls = new List<Directional>();
    [System.NonSerialized] public List<Room> adjacent_rooms = new List<Room>();


    public Room()
        : base() { }

    public Room(Building owner, Container container)
        : base(container)
    {
        this.owner = owner;
        AssignFloors();
        AssignWalls();
    }

    private void AssignFloors()
    {
        for (int x = 0; x < dimension.x; x++)
            for (int y = 0; y < dimension.y; y++)
                floors.Add(new Container(this, new Point(x, y), new Point(1, 1)));
    }

    private void AssignWalls()
    {
        for (int x = 0; x < dimension.x; x++)
        {
            walls.Add(new Directional(this, new Point(x, 0), Direction.South));
            walls.Add(new Directional(this, new Point(x, dimension.y - 1), Direction.North));
        }
        for (int y = 0; y < dimension.y; y++)
        {
            walls.Add(new Directional(this, new Point(0, y), Direction.West));
            walls.Add(new Directional(this, new Point(dimension.x - 1, y), Direction.East));
        }
    }

    public void Interpret(List<Room> rooms)
    {
        foreach (Room other_room in rooms)
            if (this.Touching(other_room))
                this.adjacent_rooms.Add(other_room);

        foreach (Room other_room in adjacent_rooms)
        {
            if (!adjacent_walls.ContainsKey(other_room))
                adjacent_walls[other_room] = new List<DirectionalPair>();

            // compute adjacent walls
            foreach (Directional this_wall in this.walls)
                foreach (Directional other_wall in other_room.walls)
                    if (this_wall.OtherSide() == other_wall)
                        adjacent_walls[other_room].Add(new DirectionalPair(this_wall, other_wall));

        }
        // compute isolated walls
        isolated_walls = new List<Directional>(this.walls); // start with all the walls.
        foreach (List<DirectionalPair> list_pair in adjacent_walls.Values)
            foreach (DirectionalPair pair in list_pair)
                isolated_walls.Remove(pair.first);
    }

    public bool MakeEntranceAgainstOtherRoom(Room other)
    {
        if (adjacent_walls[other].Count == 0)
            return false;
        Direction remove_dir = adjacent_walls[other][Random.Range(0, adjacent_walls[other].Count)].first.direction;
        DirectionalPair to_remove = CenterWall(adjacent_walls[other], remove_dir);
        this.walls.Remove(to_remove.first);
        other.walls.Remove(to_remove.second);
        this.entrances.Add(to_remove.first);
        other.entrances.Add(to_remove.second);
        return true;
    }

    public void MakeEntranceNotAgainstAnyRoom(List<Room> rooms)
    {
        if (isolated_walls.Count > 0)
        {
            Direction remove_dir = isolated_walls[Random.Range(0, isolated_walls.Count)].direction;
            Directional to_remove = CenterWall(isolated_walls, remove_dir);
            this.walls.Remove(to_remove);
            this.entrances.Add(to_remove);
        }
    }

    public bool MakeWindowAgainstOtherRoom(Room other)
    {
        if (adjacent_walls[other].Count == 0)
            return false;
        DirectionalPair to_remove = adjacent_walls[other][Random.Range(0, adjacent_walls[other].Count)];
        this.walls.Remove(to_remove.first);
        other.walls.Remove(to_remove.second);
        this.windows.Add(to_remove.first);
        other.windows.Add(to_remove.second);
        return true;
    }

    public void MakeWindowNotAgainstAnyRoom(List<Room> rooms)
    {
        if (isolated_walls.Count > 0)
        {
            Directional to_remove = isolated_walls[Random.Range(0, isolated_walls.Count)];
            this.walls.Remove(to_remove);
            this.windows.Add(to_remove);
        }
    }

    private Directional CenterWall(List<Directional> walls, Direction dir)
    {
        if (walls.Count == 0)
            return null;
        List<Directional> all_walls_same_direction = new List<Directional>();
        foreach (Directional wall in walls)
            if (wall.direction == dir)
                all_walls_same_direction.Add(wall);
        return all_walls_same_direction[all_walls_same_direction.Count/2];
    }

    private DirectionalPair CenterWall(List<DirectionalPair> wall_pairs, Direction dir)
    {
        if (wall_pairs.Count == 0)
            return new DirectionalPair();
        List<DirectionalPair> all_walls_same_direction = new List<DirectionalPair>();
        foreach (DirectionalPair wall_pair in wall_pairs)
            if (wall_pair.first.direction == dir)
                all_walls_same_direction.Add(wall_pair);
        return all_walls_same_direction[all_walls_same_direction.Count/2];
    }
    
    public override string ToString()
    {
        string to_return = "Room - Pos" + this.position_at_current_depth + " Dim" + this.dimension + " Depth(" + this.depth + ")";
        to_return += "\n Depth at Building: Pos" + this.Position(Depth.Building);
        return to_return;
    }
}
