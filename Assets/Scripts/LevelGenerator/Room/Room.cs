using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// A container that's used for breaking up lists into the 4 cardinal directions and display them on the Unity Editor.
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
public abstract class Room : MonoBehaviour
{
    private Dictionary<Direction, int> _available_entrances_d;

    public int width;
    public int height;

    public Vec2Container RelativeEntrances;
    public Vec2Container RelativeWalls;

    public bool is_spawn;

    public List<Vector2> GetEntrances()
    {
        return RelativeEntrances.Get((int)transform.position.x, (int)transform.position.y);
    }
    public List<Vector2> GetEntrances(Direction d)
    {
        return RelativeEntrances.Get((int)transform.position.x, (int)transform.position.y, d);
    }
    public List<Vector2> GetWalls()
    {
        return RelativeWalls.Get((int)transform.position.x, (int)transform.position.y);
    }
    public List<Vector2> GetWalls(Direction d)
    {
        return RelativeWalls.Get((int)transform.position.x, (int)transform.position.y, d);
    }
    
    public static Direction Opposite(Direction d)
    {
        return (Direction)(((int)d + 2) % 4);
    }
    public static Vector2 Offset(Direction d)
    {
        switch (d)
        {
            case Direction.North:
                return Vector2.up;
            case Direction.East:
                return Vector2.right;
            case Direction.South:
                return Vector2.down;
            case Direction.West:
                return Vector2.left;
        }
        return Vector2.zero;
    }

    public virtual void Initialize()
    {
        _available_entrances_d = new Dictionary<Direction, int>();
        foreach (Direction d in System.Enum.GetValues(typeof(Direction)))
            _available_entrances_d[d] = GetEntrances(d).Count;
    }


    /// <summary>
    /// This room will attempt to put itself logically into the list of current_rooms. This involves moving itself
    /// around each room while aligning entrances with each other. If this returns true, it means its current
    /// position it is leaving off is a valid spot.
    /// <para>If this returns false, make sure you delete the room or clean it up</para>
    /// </summary>
    /// <returns></returns>
    public bool Add(List<Room> current_rooms)
    {
        foreach (Room room in current_rooms)
            if (TryAlign(current_rooms, room))
                return true;
        return false;
    }

    /// <summary>
    /// Try to align this room to a room specified by "other". This means we want to try matching each permutation of entrances together.
    /// After finding an alignment, we also check if any entrances become blocked due to the new alignment.
    /// If there an entrance becomes blocked, this is an unacceptable alignment.
    /// </summary>
    /// <param name="other"></param>
    private bool TryAlign(List<Room> current_rooms, Room other)
    {
        if (other.AvailableEntrances() != 0)
        {
            foreach (Direction d in System.Enum.GetValues(typeof(Direction)))
            {
                if (other.AvailableEntrances(Opposite(d)) != 0)
                {
                    List<Vector2> this_entrances = this.GetEntrances(d);
                    List<Vector2> other_entrances = other.GetEntrances(Opposite(d));
                    for (int this_entrance = 0; this_entrance < this_entrances.Count; this_entrance++)
                    {
                        for (int other_entrance = 0; other_entrance < other_entrances.Count; other_entrance++)
                        {
                            Vector2 diff = other_entrances[other_entrance] - this_entrances[this_entrance] + Offset(Opposite(d));
                            transform.position = new Vector3(transform.position.x + diff.x,
                                                                transform.position.y + diff.y,
                                                                transform.position.z);
                            if (!Overlaps(current_rooms) && !EntranceBlocked(current_rooms) && !BlocksEntrance(current_rooms))
                            {
                                return true;
                            }
                            this_entrances = this.GetEntrances(d);
                            other_entrances = other.GetEntrances(Opposite(d));
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Updates the available entrances of this room. Checks against every other room.
    /// <para>This is an O(n) operation, n being the number of rooms</para>
    /// </summary>
    public void UpdateAvailableEntrances(List<Room> current_rooms)
    {
        foreach (Direction d in System.Enum.GetValues(typeof(Direction)))
            _available_entrances_d[d] = GetEntrances(d).Count;
        foreach (Room other in current_rooms)
            foreach (Direction d in System.Enum.GetValues(typeof(Direction)))
                foreach (Vector2 this_entrance in this.GetEntrances(d))
                    if (other.GetEntrances(Opposite(d)).Contains(this_entrance + Offset(d)))
                        _available_entrances_d[d]--;
    }
    public int AvailableEntrances()
    {
        int to_return = 0;
        foreach (Direction d in System.Enum.GetValues(typeof(Direction)))
            to_return += _available_entrances_d[d];
        return to_return;
    }
    private int AvailableEntrances(Direction d)
    {
        return _available_entrances_d[d];
    }

    /// <summary>
    /// Returns whether a given room overlaps with any of the current rooms
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public bool Overlaps(List<Room> current_rooms)
    {
        foreach (Room room in current_rooms)
            if (this.Overlaps(room))
                return true;
        return false;
    }
    /// <summary>
    /// Returns whether this room overlaps with a room specified by other.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(Room other)
    {
        if (other.GetType() == typeof(RectRoom)) // We want to do this for every different type (subclass) of Room
            return this.Overlaps((RectRoom)other);
        if (other.GetType() == typeof(IrregularRoom))
            return this.Overlaps((IrregularRoom)other);
        throw new NotImplementedException();
    }
    public abstract bool Overlaps(RectRoom other);
    public abstract bool Overlaps(IrregularRoom other);

    private bool EntranceBlocked(Room other) // Helper function. Check if any of this room's entrances are blocked by the wall specified by "other".
    {
        foreach (Direction d in System.Enum.GetValues(typeof(Direction)))
            foreach (Vector2 this_entrance in this.GetEntrances(d))
                if (other.GetWalls(Opposite(d)).Contains(this_entrance + Offset(d)))
                    return true;
        return false;   
    }
    private bool EntranceBlocked(List<Room> current_rooms) // Helper function. Check if any of this room's entrances are blocked by an existing room's wall.
    {
        foreach (Room room in current_rooms)
            if (EntranceBlocked(room))
                return true;
        return false;
    }
    private bool BlocksEntrance(List<Room> current_rooms) // Helper function. Check if any walls of this room blocks another's entrance.
    {
        foreach (Room room in current_rooms)
            if (room.EntranceBlocked(this))
                return true;
        return false;
    }
}