using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Server : NetworkManager
{
    public NetworkConnection[] connections = new NetworkConnection[10];
	void Awake()
    {
        RegisterPrefabs();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 200, 100, 20), "Connections: " + numPlayers);
        for (int i = 0; i < connections.Length; i++)
            if (connections[i] != null)
                GUI.Label(new Rect(0, 220 + 20 * i, 300, 20), connections[i].ToString());
            else
                GUI.Label(new Rect(0, 220 + 20 * i, 300, 20), "----- No Player -----");
    }

    private void RegisterPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Rooms");
        foreach (GameObject g in prefabs)
            ClientScene.RegisterPrefab(g);
        prefabs = Resources.LoadAll<GameObject>("SpawnRooms");
        foreach (GameObject g in prefabs)
            ClientScene.RegisterPrefab(g);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Connect from: " + conn.ToString());
        connections[conn.connectionId] = conn;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Disconnect from: " + conn.ToString());
        connections[conn.connectionId] = null;
    }

}
