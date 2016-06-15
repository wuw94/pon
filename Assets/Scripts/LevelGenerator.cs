using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelGenerator : MonoBehaviour {

    public int AccelerateUntil = 5; // while our gameobjects is less than this amount, we can't close entrances
    public int DecelerateAt = 5; // once our gameobjects hits this amount, we need to start closing entrances
    public GameObject tile;
    public Room[] room_lib;
    public static List<Room> current_rooms = new List<Room>();
    public bool done = false;

    //public Tuple<int, int> s = new Tuple<int, int>();

    private void Awake()
    {
        LoadResources();
        CreateSpawn();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateRoom();
        }
        if (Input.GetKeyDown(KeyCode.Z))
            Debug.Log(AvailableEntrances());
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = new Vector2((int)Camera.main.ScreenToWorldPoint(Input.mousePosition).x, (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        }
        CreateRoom();
    }

    private void LoadResources()
    {
        LoadRooms();
    }

    private void LoadRooms()
    {
        GameObject[] rooms = Resources.LoadAll<GameObject>("Rooms");
        room_lib = new Room[rooms.Length];
        for (int i = 0; i < rooms.Length; i++)
        {
            room_lib[i] = rooms[i].GetComponent<Room>();
        }
    }

    private void CreateSpawn()
    {
        Room r = Instantiate<Room>(room_lib[1]);
        r.transform.position = new Vector3(2, 2, 0);
        current_rooms.Add(r);
    }

    private void CreateRoom()
    {
        // Create a random room from our room library
        if (!done && AvailableEntrances() > 0)
        {
            Room r = Instantiate<Room>(room_lib[UnityEngine.Random.Range(0, room_lib.Length)]);

            // while we're accelerating, we don't want rooms with less entrances than 2
            if (current_rooms.Count < AccelerateUntil && r.GetEntrances().Count < 2)
            {
                Destroy(r.gameObject);
                return;
            }

            // while we're decelerating, we don't want rooms with more than 2 entrances
            // TODO: There's cases where the only possible room to place needs to have more than 2 entrances
            if (current_rooms.Count >= DecelerateAt && r.GetEntrances().Count > 2)
            {
                Destroy(r.gameObject);
                return;
            }

            else if (!r.Add())
            {
                Destroy(r.gameObject);
                return;
            }
        }
        else
        {
            done = true;
        }

    }
    
    public int AvailableEntrances()
    {
        int to_return = 0;
        foreach (Room room in current_rooms)
        {
            room.UpdateAvailableEntrances();
            to_return += room.AvailableEntrances;
        }
        return to_return;
    }

}
