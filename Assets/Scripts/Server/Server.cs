using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public enum MenuPage
{
    Home, Creating, Listing, Joining
}

/// <summary>
/// Server.
/// The Server class derives from Unity's recent NetworkManager class, granting
/// the high level API functions which allow hosting and joining rooms.
/// 
/// auth Wesley Wu
/// </summary>
public class Server : NetworkManager
{
    [HideInInspector]
    public string player_name = "Boring Name";
    //private string try_ip;

    /// <summary>
    /// Our list of connections (in int based on a connection's connectionId).
    /// </summary>
    [HideInInspector]
    public int[] connections = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

    public MenuPage current_page = MenuPage.Home;

    private List<MatchInfoSnapshot> match_snapshots = new List<MatchInfoSnapshot>();

    

    private void Start()
    {
        NetworkManager.singleton.StartMatchMaker();
        StartCoroutine(RefreshListing());
    }


    private IEnumerator RefreshListing()
    {
        while (SceneManager.GetActiveScene().name == offlineScene)
        {
            if (current_page == MenuPage.Listing)
                ListInternetMatches();
            yield return new WaitForSeconds(1);
        }
    }


    /// <summary>
    /// Stuff to do to each player for them to acknowledge the
    /// presence of a new connecting player
    /// </summary>
    private void UpdateConnections()
    {
        GUIShowPlayers[] g = FindObjectsOfType<GUIShowPlayers>();
        for (int i = 0; i < g.Length; i++)
        {
            int id = g[i].connectionToClient.connectionId;
            g[i].RpcUpdateConnections(connections, id);
        }
    }


    /// <summary>
    /// We need to register the prefabs before it is able to be
    /// instantiated on the network.
    /// </summary>
    private void RegisterPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("");
        foreach (GameObject g in prefabs)
            if (g.GetComponent<NetworkIdentity>() != null)
                ClientScene.RegisterPrefab(g);
    }

    

    
    private void OnGUI()
    {
        /*
        GUISkin gskn = Resources.Load<GUISkin>("GUI Skins/Menu");
        GUI.BeginGroup(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300), gskn.FindStyle("Title Screen"));
        GUI.Box(new Rect(0, 0, 300, 300), "This is the title of a box", gskn.FindStyle("Title Screen"));
        GUI.Button(new Rect(0, 25, 100, 20), "I am a button", gskn.FindStyle("Title Screen"));
        GUI.Label(new Rect(0, 50, 100, 20), "I'm a Label!", gskn.FindStyle("Title Screen"));
        GUI.EndGroup();
        */

        // ________________________------------------------
        if (SceneManager.GetActiveScene().name == offlineScene)
        {
            if (current_page == MenuPage.Home)
            {
                // Your Name
                GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 - 20, 150, 20), "Your Name");
                player_name = GUI.TextField(new Rect(Screen.width / 2, Screen.height / 2, 150, 20), player_name, 15);

                // Create Game
                if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 40, 150, 20), "Create Match") && player_name != "")
                    CreateInternetMatch();

                // Join Game
                if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 60, 150, 20), "Join Match") && player_name != "")
                {
                    current_page = MenuPage.Listing;
                    ListInternetMatches();
                }
            }

            if (current_page == MenuPage.Listing)
            {
                GUI.Label(new Rect(10, 20, 300, 20), "Hello [" + player_name + "]. Here are your matches");
                if (GUI.Button(new Rect(300,20, 60, 20), "Back"))
                {
                    current_page = MenuPage.Home;
                }
                for (int i = 0; i < match_snapshots.Count; i++)
                {
                    if (GUI.Button(new Rect(10, 40 + 20 * i, 300, 20), match_snapshots[i].name + " " + match_snapshots[i].currentSize + "/" + match_snapshots[i].maxSize))
                    {
                        JoinInternetMatch(match_snapshots[i]);
                    }
                }
            }

            // Make Local Game (For Debugging)
            if (Input.GetKey(KeyCode.RightControl))
            {
                GUI.Label(new Rect(Screen.width - 200, Screen.height - 20, 200, 20), "For debugging. No touchie!");
                if (GUI.Button(new Rect(Screen.width - 40, Screen.height - 20, 20, 20), "C"))
                {
                    player_name = "host";
                    StartHost();
                }
                if (GUI.Button(new Rect(Screen.width - 20, Screen.height - 20, 20, 20), "J"))
                {
                    player_name = "client";
                    networkAddress = "localhost";
                    StartClient();
                }
            }
        }

    }

    // ------------------------------------------------- Matchmaking Handlers -------------------------------------------------

    //call this method to request a match to be created on the server
    private void CreateInternetMatch()
    {
        string match_name = player_name + "'s Game";
        NetworkManager.singleton.matchMaker.CreateMatch(match_name, matchSize, true, "", "", "", 0, 0, OnInternetMatchCreate);
    }

    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo responseData)
    {
        if (success)
        {
            MatchInfo hostInfo = responseData;
            NetworkServer.Listen(hostInfo, 9000);
            NetworkManager.singleton.StartHost(hostInfo);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    private void ListInternetMatches()
    {
        NetworkManager.singleton.matchMaker.ListMatches(0, 5, "", false, 0, 0, OnInternetMatchList);
    }

    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
    {
        if (success)
            match_snapshots = new List<MatchInfoSnapshot>();
            if (responseData.Count != 0)
                foreach (MatchInfoSnapshot m_info in responseData)
                    match_snapshots.Add(m_info);
    }


    private void JoinInternetMatch(MatchInfoSnapshot mis)
    {
        NetworkManager.singleton.matchMaker.JoinMatch(mis.networkId, "", "", "", 0, 0, OnInternetMatchJoin);
    }

    private void OnInternetMatchJoin(bool success, string extendedInfo, MatchInfo responseData)
    {
        if (success)
        {
            Debug.Log("Able to join a match");
            MatchInfo hostInfo = responseData;
            NetworkManager.singleton.StartClient(hostInfo);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }


    // ------------------------------------------------- Unity Overrides -------------------------------------------------
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
        connections[conn.connectionId] = conn.connectionId;
        UpdateConnections();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect");
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
        NetworkServer.SpawnObjects();
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

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        Debug.Log("OnMatchCreate");
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
        Debug.Log("OnMatchJoined");
    }

    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        base.OnMatchList(success, extendedInfo, matchList);
        Debug.Log("OnMatchList");
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
