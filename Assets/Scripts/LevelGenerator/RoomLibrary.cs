using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// A room library is a Dictionary of a Dictionary of a List of Rooms
/// Give it a specified entrance number, a dimension of room you want, and which room you want in that list
/// <para>Get a random room with GetRandom(int entrances, Vector2 size)</para>
/// </summary>
public class RoomLibrary
{
    private Dictionary<int, Dictionary<Vector2, List<Room>>> _room_lib;
    public int entrance_min = Int32.MaxValue;
    public int entrance_max = 0;
    public int size_min = Int32.MaxValue;
    public int size_max = 0;

    /// <summary>
    /// Constructor.
    /// </summary>
    public RoomLibrary()
    {
        _room_lib = new Dictionary<int, Dictionary<Vector2, List<Room>>>();
    }
    /// <summary>
    /// Clears the RoomLibrary. Be careful when using this!
    /// </summary>
    public void ClearAll()
    {
        _room_lib = new Dictionary<int, Dictionary<Vector2, List<Room>>>();
    }
    /// <summary>
    /// Add a room to the RoomLibrary. The RoomLibrary will analyze the number of
    /// entrances and size for you.
    /// 
    /// <para>TODO: This creates and deletes a temporary game object for analysis. This needs to change in the future</para>
    /// </summary>
    /// <param name="r"></param>
    public void Add(Room r)
    {
        Room temp = UnityEngine.Object.Instantiate<Room>(r);
        int entrances = temp.GetEntrances().Count;
        UnityEngine.Object.Destroy(temp.gameObject);

        Vector2 size = new Vector2(r.width, r.height);

        if (!_room_lib.ContainsKey(entrances))
            _room_lib[entrances] = new Dictionary<Vector2, List<Room>>();
        if (!_room_lib[entrances].ContainsKey(size))
            _room_lib[entrances][size] = new List<Room>();
        if (entrances < entrance_min)
            entrance_min = entrances;
        if (entrances > entrance_max)
            entrance_max = entrances;
        if (size.x < size_min)
            size_min = (int)size.x;
        if (size.x > size_max)
            size_max = (int)size.x;
        _room_lib[entrances][size].Add(r);
    }
    /// <summary>
    /// Get us a random room from the RoomLibrary with the specifications.
    /// Returns null if nothing is found.
    /// </summary>
    /// <param name="entrances"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Room GetRandom(int entrances, Vector2 size)
    {
        if (!_room_lib.ContainsKey(entrances))
            return null;
        if (!_room_lib[entrances].ContainsKey(size))
            return null;
        if (_room_lib[entrances][size].Count == 0)
            return null;
        return _room_lib[entrances][size][UnityEngine.Random.Range(0, _room_lib[entrances][size].Count)];
    }

    public List<Room> Get(int entrances)
    {
        List<Room> to_return = new List<Room>();
        if (!_room_lib.ContainsKey(entrances))
            return null;
        foreach (KeyValuePair<Vector2, List<Room>> layer2 in _room_lib[entrances])
        {
            foreach (Room room in layer2.Value)
            {
                to_return.Add(room);
            }
        }
        return to_return;
    }

    public Room GetRandomWithinSize(bool accelerate, int size_min, int size_max)
    {
        foreach (KeyValuePair<int, Dictionary<Vector2, List<Room>>> layer1 in _room_lib)
        {
            if (layer1.Key > 2)
            {
                foreach (KeyValuePair<Vector2, List<Room>> layer2 in layer1.Value)
                {
                    if (layer2.Key.x >= size_min && layer2.Key.x <= size_max)
                    {
                        return GetRandom(layer1.Key, layer2.Key);
                    }
                }
            }

        }
        return null;
    }

    public Room GetRandom(bool accelerate, Vector2 size)
    {
        if (accelerate)
            for (int i = 3; i < 100; i++)
                if (GetRandom(i, size) != null)
                    return _room_lib[i][size][UnityEngine.Random.Range(0, _room_lib[i][size].Count)];
        if (!accelerate)
            for (int i = 0; i < 100; i++)
                if (GetRandom(i, size) != null)
                    return _room_lib[i][size][UnityEngine.Random.Range(0, _room_lib[i][size].Count)];
        return null;
    }

    public override string ToString()
    {
        int _total_rooms = 0;
        System.Text.StringBuilder _to_return = new System.Text.StringBuilder();
        _to_return.Append(_room_lib.Keys.Count.ToString() + " unique number of entrances\n");
        foreach (KeyValuePair<int, Dictionary<Vector2, List<Room>>> layer1 in _room_lib)
        {
            _to_return.Append("    Entrances: [" + layer1.Key.ToString() + "], " + layer1.Value.Keys.Count.ToString() + " unique dimensions\n");
            foreach (KeyValuePair<Vector2, List<Room>> layer2 in layer1.Value)
            {
                _to_return.Append("        Dimensions: [x:" + layer2.Key.x.ToString() + ", y:" + layer2.Key.y.ToString() + "] - ");
                _to_return.Append(layer2.Value.Count.ToString() + " of these\n");
                _total_rooms += layer2.Value.Count;
            }
        }
        return " RoomLibrary: " + _total_rooms.ToString() + " rooms total\n\n" + _to_return.ToString();
    }
}