using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Server.
/// The Server class derives from Unity's recent NetworkManager class, granting
/// the high level API functions which allow hosting and joining rooms.
/// 
/// auth Wesley Wu
/// </summary>
public class Server : NetworkManager
{
    /// <summary>
    /// Our list of connections (in int based on a connection's connectionId).
    /// </summary>
    public int[] connections = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };


    private void Update()
    {
    }

    /// <summary>
    /// Stuff to do to each player for them to acknowledge the
    /// presence of a new connecting player
    /// </summary>
    private void UpdateConnections()
    {
        PlayerInfo[] p = FindObjectsOfType<PlayerInfo>();
        for (int i = 0; i < p.Length; i++)
        {
            int id = p[i].connectionToClient.connectionId;
            p[i].RpcUpdateConnections(connections, id);
            p[i].RpcUpdateTeam(id);
        }
    }


    /// <summary>
    /// We need to register the prefabs before it is able to be
    /// instantiated on the network.
    /// </summary>
    private void RegisterPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Rooms");
        foreach (GameObject g in prefabs)
            ClientScene.RegisterPrefab(g);
        prefabs = Resources.LoadAll<GameObject>("SpawnRooms");
        foreach (GameObject g in prefabs)
            ClientScene.RegisterPrefab(g);
        prefabs = Resources.LoadAll<GameObject>("Damagers");
        foreach (GameObject g in prefabs)
            ClientScene.RegisterPrefab(g);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect");
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("OnClientError");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log("OnClientDisconnect");
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        base.OnClientNotReady(conn);
        Debug.Log("OnClientNotReady");
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        Debug.Log("OnClientSceneChanged");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        Debug.Log("OnServerAddPlayer");
        UpdateConnections();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Connect from: " + conn.ToString());
        connections[conn.connectionId] = conn.connectionId;
        
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Disconnect from: " + conn.ToString());
        connections[conn.connectionId] = -1;
        UpdateConnections();
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);
        Debug.Log("OnServerError");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.Log("OnServerReady");
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        Debug.Log("OnServerRemovePlayer");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Debug.Log("OnServerSceneChanged");
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        
        Debug.Log("OnStartClient");
        RegisterPrefabs();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("OnStartHost");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("OnStopClient");
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.Log("OnStopHost");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("OnStopServer");
    }

}
