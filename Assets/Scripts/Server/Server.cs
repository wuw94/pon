using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Server.
/// The Server class derives from Unity's recent NetworkManager class, granting
/// the high level API functions which allow hosting and joining rooms.
/// 
/// auth Wesley Wu
/// </summary>
public class Server : NetworkManager
{
    private void Start()
    {
        StartMatchMaker();
        //StartCoroutine(RefreshListing());
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == offlineScene)
        {
            if (MenuManager.current_menu == typeof(MenuPreGameListing))
                ListInternetMatches();
        }
    }

    public void ExitMatchToHome()
    {
        if (SceneManager.GetActiveScene().name == offlineScene)
        {
            Debug.LogError("Calling in wrong scene");
            return;
        }
        StopHost();
        StopMatchMaker();
        MenuManager.current_menu = typeof(MenuPreGameHome);
        StartMatchMaker();
    }

    private IEnumerator RefreshListing()
    {
        while (SceneManager.GetActiveScene().name == offlineScene)
        {
            if (MenuManager.current_menu == typeof(MenuPreGameListing))
                ListInternetMatches();
            yield return new WaitForSeconds(1);
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

    

    
    

    // ------------------------------------------------- Matchmaking Handlers -------------------------------------------------

    //call this method to request a match to be created on the server
    public void CreateInternetMatch(string player_name)
    {
        string match_name = player_name + "'s Game";
        matchMaker.CreateMatch(match_name, matchSize, true, "", "", "", 0, 0, OnMatchCreate);
    }

    public void ListInternetMatches()
    {
        matchMaker.ListMatches(0, 5, "", false, 0, 0, OnMatchList);
    }

    public void JoinInternetMatch(MatchInfoSnapshot mis)
    {
        matchMaker.JoinMatch(mis.networkId, "", "", "", 0, 0, OnMatchJoined);
    }

    public void DestroyInternetMatch()
    {
        Debug.Log("DestroyMatch");
        matchMaker.DestroyMatch(matchInfo.networkId, matchInfo.domain, OnDestroyMatch);
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
        ExitMatchToHome();
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
        // Somebody joined the game
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
        // Somebody exited the game
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
        //base.OnMatchCreate(success, extendedInfo, matchInfo);
        if (success)
        {
            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);
            StartHost(hostInfo);
            Debug.Log(this.matchName);
        }
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

    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        base.OnDestroyMatch(success, extendedInfo);
        // This callback is never called.
        Debug.Log("OnDestroyMatch - success=" +success);

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
