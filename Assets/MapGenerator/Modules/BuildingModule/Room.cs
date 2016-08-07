using UnityEngine;
using System.Collections.Generic;

public class Room : Container
{
    public HashSet<Point> floors = new HashSet<Point>();
    public HashSet<Wall> walls = new HashSet<Wall>();
    public HashSet<Point> entrances = new HashSet<Point>();

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
                floors.Add(new Point(x, y));
    }

    private void AssignWalls()
    {
        for (int x = 0; x < dimension.x; x++)
        {
            walls.Add(new Wall(this, new Point(x, 0), Direction.South));
            walls.Add(new Wall(this, new Point(x, dimension.y - 1), Direction.North));
        }
        for (int y = 0; y < dimension.y; y++)
        {
            walls.Add(new Wall(this, new Point(0, y), Direction.West));
            walls.Add(new Wall(this, new Point(dimension.x - 1, y), Direction.East));
        }
    }

    public bool MakeEntranceAgainstOtherRoom(Room other)
    {
        List<WallPair> touching_walls = this.TouchingWalls(other);

        if (touching_walls.Count > 0)
        {
            WallPair to_remove = touching_walls[UnityEngine.Random.Range(0, touching_walls.Count)];
            walls.Remove(to_remove.first);
            other.walls.Remove(to_remove.second);
            return true;
        }
        return false;
    }


    /// <summary>
    /// Returns a HashSet of pairs of touching walls between two HashSets.
    /// </summary>
    /// <param name="this_walls"></param>
    /// <param name="other_walls"></param>
    /// <returns></returns>
    private List<WallPair> TouchingWalls(Room other_room)
    {
        List<WallPair> to_return = new List<WallPair>();
        foreach (Wall this_wall in this.walls)
            foreach (Wall other_wall in other_room.walls)
                if (this_wall.OtherSide() == other_wall)
                    to_return.Add(new WallPair(this_wall, other_wall));
        return to_return;
    }

    /// <summary>
    /// Returns a HashSet of pairs of walls that touch between this room and a list of all rooms.
    /// </summary>
    /// <param name="this_room"></param>
    /// <param name="all_rooms"></param>
    /// <returns></returns>
    private List<WallPair> TouchingWalls(List<Room> all_rooms)
    {
        List<WallPair> to_return = new List<WallPair>();
        foreach (Room other_room in all_rooms)
            to_return.AddRange(this.TouchingWalls(other_room));
        return to_return;
    }

    public void MakeEntranceNotAgainstAnyRoom(List<Room> rooms)
    {
        List<Wall> non_touching_walls = new List<Wall>(this.NonTouchingWalls(rooms));
        if (non_touching_walls.Count > 0)
            walls.Remove(non_touching_walls[UnityEngine.Random.Range(0, non_touching_walls.Count)]);
    }


    /// <summary>
    /// Returns a HashSet of all walls in a room that aren't touching walls of any other rooms.
    /// </summary>
    /// <param name="this_room"></param>
    /// <param name="all_rooms"></param>
    /// <returns></returns>
    private List<Wall> NonTouchingWalls(List<Room> all_rooms)
    {
        List<Wall> to_return = new List<Wall>(this.walls); // start with all the walls.
        foreach (WallPair pair in this.TouchingWalls(all_rooms))
            to_return.Remove(pair.first);
        return to_return;
    }

    public override string ToString()
    {
        string to_return = "Room - Pos" + this.position_at_current_depth + " Dim" + this.dimension + " Depth(" + this.depth + ")";
        to_return += "\n Depth at Building: Pos" + this.Position(Depth.Building);
        return to_return;
    }
}
