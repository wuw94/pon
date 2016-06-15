using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A container that's used for breaking up lists into the 4 cardinal directions.
/// <para>North, East, South, West</para>
/// </summary>
[System.Serializable]
public class Vec2Container
{
    [SerializeField]
    private Vector2[] North;
    [SerializeField]
    private Vector2[] East;
    [SerializeField]
    private Vector2[] South;
    [SerializeField]
    private Vector2[] West;

    public List<Vector2> Get(int offsetX, int offsetY)
    {
        List<Vector2> to_return = new List<Vector2>();
        foreach (Vector2 entrance in North)
            to_return.Add(entrance + new Vector2(offsetX, offsetY));
        foreach (Vector2 entrance in East)
            to_return.Add(entrance + new Vector2(offsetX, offsetY));
        foreach (Vector2 entrance in South)
            to_return.Add(entrance + new Vector2(offsetX, offsetY));
        foreach (Vector2 entrance in West)
            to_return.Add(entrance + new Vector2(offsetX, offsetY));
        return to_return;
    }
    public List<Vector2> Get(int offsetX, int offsetY, Direction d)
    {
        List<Vector2> to_return = new List<Vector2>();
        if (d == Direction.North)
            foreach (Vector2 entrance in North)
                to_return.Add(entrance + new Vector2(offsetX, offsetY));
        else if (d == Direction.East)
            foreach (Vector2 entrance in East)
                to_return.Add(entrance + new Vector2(offsetX, offsetY));
        else if (d == Direction.South)
            foreach (Vector2 entrance in South)
                to_return.Add(entrance + new Vector2(offsetX, offsetY));
        else
            foreach (Vector2 entrance in West)
                to_return.Add(entrance + new Vector2(offsetX, offsetY));
        return to_return;
    }
}

/* Room.
 * 
 * Details:
 *  A room script to be attached to a GameObject. If you want to access details of a room, use this component in the GameObject.
 *  LevelGenerator.current_rooms - gives us all the rooms in the level.
 * 
 * Technicals:
 *  The bottom left of a room is its relative center (0,0).
 *  Coordinates of everything is based on its center, the bottom left position.
 *  Entrances entered in the prefabs are relative to the bottom right corner of a room.
 *  For consistency measures, all the Get() functions will return a room's details in absolute values, not relative.
 *  Room operations will NEVER change the details of another room.
 *  Room details are specified in the prefabs. Some wacky stuff might happen if you make negative size rooms.
 *  
 * Public Gets:
 *  width   height
 *  minX    maxX
 *  minY    maxY
 *  GetEntrances()          GetEntrances(Direction d)
 *  GetWalls()              GetWalls(Direction d)
 *  Overlaps(Room other)
 * 
 * Public Methods:
 *  Add()
 * 
 * auth Wesley Wu
 */
public class Room : MonoBehaviour
{
    public int width;
    public int height;

    public int minX { get { return (int)transform.position.x; } }
    public int maxX { get { return (int)transform.position.x + width - 1; } }
    public int minY { get { return (int)transform.position.y; } }
    public int maxY { get { return (int)transform.position.y + height - 1; } }

    public Vec2Container RelativeEntrances;
    public Vec2Container RelativeWalls;
    public int AvailableEntrances;

    public List<Vector2> GetEntrances()
    {
        return RelativeEntrances.Get(minX, minY);
    }
    public List<Vector2> GetEntrances(Direction d)
    {
        return RelativeEntrances.Get(minX, minY, d);
    }
    public List<Vector2> GetWalls()
    {
        return RelativeWalls.Get(minX, minY);
    }
    public List<Vector2> GetWalls(Direction d)
    {
        return RelativeWalls.Get(minX, minY, d);
    }
    
    public Vector2 Relative2Absolute(Vector2 relative_coordinate)
    {
        return new Vector2(relative_coordinate.x + minX, relative_coordinate.y + minY);
    }
    public Vector2 Absolute2Relative(Vector2 absolute_coordinate)
    {
        return new Vector2(absolute_coordinate.x - minX, absolute_coordinate.y - minY);
    }

    private void Awake()
    {
        AvailableEntrances = GetEntrances().Count;
    }

    private void OnMouseDown()
    {
        Debug.Log(this);
    }

    public void UpdateAvailableEntrances()
    {
        AvailableEntrances = GetEntrances().Count;
        foreach (Room other in LevelGenerator.current_rooms)
        {
            foreach (Vector2 this_entrance in this.GetEntrances(Direction.North))
                if (other.GetEntrances(Direction.South).Contains(this_entrance + Vector2.up))
                    AvailableEntrances--;
            foreach (Vector2 this_entrance in this.GetEntrances(Direction.East))
                if (other.GetEntrances(Direction.West).Contains(this_entrance + Vector2.right))
                    AvailableEntrances--;
            foreach (Vector2 this_entrance in this.GetEntrances(Direction.South))
                if (other.GetEntrances(Direction.North).Contains(this_entrance + Vector2.down))
                    AvailableEntrances--;
            foreach (Vector2 this_entrance in this.GetEntrances(Direction.West))
                if (other.GetEntrances(Direction.East).Contains(this_entrance + Vector2.left))
                    AvailableEntrances--;
        }
    }


    /// <summary>
    /// Returns whether a given room overlaps with any of the current rooms
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public bool Overlaps()
    {
        foreach (Room room in LevelGenerator.current_rooms)
            if (this.Overlaps(room))
                return true;
        return false;
    }
    // Caution! This checks overlaps in assumption that your room is a rectangle shape
    /// <summary>
    /// Returns whether this room overlaps with a room specified by other.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Room other)
    {
        return this.minX <= other.maxX && other.minX <= this.maxX && this.minY <= other.maxY && other.minY <= this.maxY;
    }

    /// <summary>
    /// This room will attempt to put itself logically into the list of current_rooms.
    /// If it can, it will put itself into the list.
    /// If it can't, it will delete itself.
    /// </summary>
    /// <returns></returns>
    public bool Add()
    {
        foreach (Room room in LevelGenerator.current_rooms)
            if (TryAlign(room))
            {
                LevelGenerator.current_rooms.Add(this);
                return true;
            }
        return false;
    }

    /// <summary>
    /// Try to align this room to a room specified by "other". This means we want to try matching each permutation of entrances together.
    /// After finding an alignment, we also check if any entrances become blocked due to the new alignment.
    /// If there an entrance becomes blocked, this is an unacceptable alignment.
    /// </summary>
    /// <param name="other"></param>
    private bool TryAlign(Room other)
    {
        if (other.AvailableEntrances == 0)
            return false;
        return TryAlignNorth2South(other) || TryAlignEast2West(other) || TryAlignSouth2North(other) || TryAlignWest2East(other);
    }
    private bool TryAlignNorth2South(Room other) // Helper function. Try to align any of this room's NORTH entrances with any SOUTH entrances specified by "other".
    {
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.North))
        {
            foreach (Vector2 other_entrance in other.GetEntrances(Direction.South))
            {
                Vector2 diff = other_entrance - this_entrance + Vector2.down;
                transform.position = new Vector3(transform.position.x + diff.x,
                                                    transform.position.y + diff.y,
                                                    transform.position.z);
                if (!Overlaps() && !EntranceBlocked() && !BlocksEntrance())
                {
                    return true;
                }
                    
            }
        }
        return false;
    }
    private bool TryAlignEast2West(Room other) // Helper function. Try to align any of this room's EAST entrances with any WEST entrances specified by "other".
    {
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.East))
        {
            foreach (Vector2 other_entrance in other.GetEntrances(Direction.West))
            {
                Vector2 diff = other_entrance - this_entrance + Vector2.left;
                transform.position = new Vector3(transform.position.x + diff.x,
                                                    transform.position.y + diff.y,
                                                    transform.position.z);
                if (!Overlaps() && !EntranceBlocked() && !BlocksEntrance())
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool TryAlignSouth2North(Room other) // Helper function. Try to align any of this room's SOUTH entrances with any NORTH entrances specified by "other".
    {
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.South))
        {
            foreach (Vector2 other_entrance in other.GetEntrances(Direction.North))
            {
                Vector2 diff = other_entrance - this_entrance + Vector2.up;
                transform.position = new Vector3(transform.position.x + diff.x,
                                                    transform.position.y + diff.y,
                                                    transform.position.z);
                if (!Overlaps() && !EntranceBlocked() && !BlocksEntrance())
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool TryAlignWest2East(Room other) // Helper function. Try to align any of this room's WEST entrances with any EAST entrances specified by "other".
    {
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.West))
        {
            foreach (Vector2 other_entrance in other.GetEntrances(Direction.East))
            {
                Vector2 diff = other_entrance - this_entrance + Vector2.right;
                transform.position = new Vector3(transform.position.x + diff.x,
                                                    transform.position.y + diff.y,
                                                    transform.position.z);
                if (!Overlaps() && !EntranceBlocked() && !BlocksEntrance())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool EntranceBlocked(Room other) // Helper function. Check if any of this room's entrances are blocked by the wall specified by "other".
    {
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.North))
            if (other.GetWalls(Direction.South).Contains(this_entrance + Vector2.up))
                return true;
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.East))
            if (other.GetWalls(Direction.West).Contains(this_entrance + Vector2.right))
                return true;
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.South))
            if (other.GetWalls(Direction.North).Contains(this_entrance + Vector2.down))
                return true;
        foreach (Vector2 this_entrance in this.GetEntrances(Direction.West))
            if (other.GetWalls(Direction.East).Contains(this_entrance + Vector2.left))
                return true;
        return false;   
    }
    private bool EntranceBlocked() // Helper function. Check if any of this room's entrances are blocked by an existing room's wall.
    {
        foreach (Room room in LevelGenerator.current_rooms)
            if (EntranceBlocked(room))
                return true;
        return false;
    }
    private bool BlocksEntrance() // Helper function. Check if any walls of this room blocks another's entrance.
    {
        foreach (Room room in LevelGenerator.current_rooms)
            if (room.EntranceBlocked(this))
                return true;
        return false;
    }
}