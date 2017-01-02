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
    /// <summary>
    /// A bool to restrict operations until network callbacks have succeeded.
    /// </summary>
    //public bool network_resolved = true;
    public bool destroy_match = false;

    public int checking_domain = 0;

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
        //Debug.Log(Settings.WAIT_FOR.Count);
    }

    public void SendToHomeMenu()
    {
        StopHost();
        StopMatchMaker();
        StartMatchMaker();
        MenuManager.current_menu = typeof(MenuPreGameHome);
    }

    // We don't call this because there's nothing running in the background, i.e. we don't need to limit the checks
    private IEnumerator RefreshListing()
    {
        while (SceneManager.GetActiveScene().name == offlineScene)
        {
            if (MenuManager.current_menu == typeof(MenuPreGameListing))
                ListInternetMatches();
            yield return new WaitForSeconds(0.2f);
        }
    }



    /// <summary>
    /// We need to register the prefabs before it is able to be instantiated on the network.
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
        string match_name = player_name + "'s Game [" + System.DateTime.Now.TimeOfDay + "]";
        matchMaker.CreateMatch(match_name, matchSize, true, "", "", "", 0, 0, OnMatchCreate);
    }

    public void ListInternetMatches()
    {
        matchMaker.ListMatches(0, 5, "", true, 0, checking_domain, OnMatchList);
    }

    public void JoinInternetMatch(MatchInfoSnapshot mis)
    {
        matchMaker.JoinMatch(mis.networkId, "", "", "", 0, checking_domain, OnMatchJoined);
    }

    public void DestroyInternetMatch()
    {
        destroy_match = true;
        UnlistMatch();
        matchMaker.DestroyMatch(matchInfo.networkId, matchInfo.domain, OnDestroyMatch);
    }

    
    public void UnlistMatch()
    {
        matchMaker.SetMatchAttributes(this.matchInfo.networkId, false, this.matchInfo.domain, OnSetMatchAttributes);
    }


    // ------------------------------------------------- UNET Overrides -------------------------------------------------
    

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("OnClientError");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log("OnClientDisconnect");
        SendToHomeMenu();
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        base.OnClientNotReady(conn);
        Debug.Log("OnClientNotReady");
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

    

    public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        Debug.Log("OnServerRemovePlayer");
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






    // ------------------------------------------------- Network Flow -------------------------------------------------


    /// <summary>
    /// This hook is called when a host is started.
    /// StartHost has multiple signatures, but they all cause this hook to be called.
    /// </summary>
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("OnStartHost");
    }

    /// <summary>
    /// This hook is called when a server is started - including when a host is started.
    /// StartServer has multiple signatures, but they all cause this hook to be called.
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer");
    }

    /// <summary>
    /// Called on the server when a new client connects.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    /// <summary>
    /// This is a hook that is invoked when the client is started.
    /// StartClient has multiple signatures, but they all cause this hook to be called.
    /// </summary>
    /// <param name="client"></param>
    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        Debug.Log("OnStartClient");
        RegisterPrefabs();
    }

    /// <summary>
    /// Callback that happens when a NetworkMatch.CreateMatch request has been processed on the server.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="matchInfo"></param>
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        this.matchInfo = matchInfo;
        Settings.WAIT_FOR.Remove(Settings.WaitTypes.CREATE_MATCH_CALLBACK);
        Settings.WAIT_FOR.Add(Settings.WaitTypes.CREATE_MATCH);
        Debug.Log("OnMatchCreate: success=" + success);
    }

    /// <summary>
    /// Called on the client when connected to a server.
    /// The default implementation of this function sets the client as ready and adds a player.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect");
    }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName"></param>
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Debug.Log("OnServerSceneChanged");
        NetworkServer.SpawnObjects();
    }

    /// <summary>
    /// Called on the server when a client is ready.
    /// The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.Log("OnServerReady");
    }

    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// The default implementation for this function creates a new player object from the playerPrefab.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        Debug.Log("OnServerAddPlayer");
        // Somebody joined the game
    }

    /// <summary>
    /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    /// Scene changes can cause player objects to be destroyed.The default implementation of OnClientSceneChanged
    /// in the NetworkManager is to add a player object for the connection if no player object exists.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        StartCoroutine(WaitForPlayer());
        Debug.Log("OnClientSceneChanged");
    }

    private IEnumerator WaitForPlayer()
    {
        while (Player.mine == null)
            yield return null;
        if (Player.mine.is_host)
            Settings.WAIT_FOR.Remove(Settings.WaitTypes.CREATE_MATCH);
        else
            Settings.WAIT_FOR.Remove(Settings.WaitTypes.JOIN_MATCH);
    }


    // When we want to destroy the match, we call matchMaker.SetMatchAttributes to make the match invisible
    // Then we call matchMaker.DestroyMatch to forcibly remove the match from the Unity master server.



    /// <summary>
    /// Callback that happens when a NetworkMatch.SetMatchAttributes has been processed on the server.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    public override void OnSetMatchAttributes(bool success, string extendedInfo)
    {
        base.OnSetMatchAttributes(success, extendedInfo);
        Debug.Log("OnSetMatchAttributes callback: success=" + success);

        if (destroy_match)
        {
            
            StopHost();
            StopMatchMaker();
            StartMatchMaker();
        }
    }

    /// <summary>
    /// This hook is called when a host is stopped.
    /// </summary>
    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.Log("OnStopHost");
    }

    /// <summary>
    /// This hook is called when a server is stopped - including when a host is stopped.
    /// </summary>
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("OnStopServer");
    }

    /// <summary>
    /// This hook is called when a client is stopped.
    /// </summary>
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("OnStopClient");
    }

    /// <summary>
    /// Callback that happens when a NetworkMatch.DestroyMatch request has been processed on the server.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        base.OnDestroyMatch(success, extendedInfo);
        Debug.Log("OnDestroyMatch: success=" + success);
        if (success)
        {
            SendToHomeMenu();
            Settings.WAIT_FOR.Remove(Settings.WaitTypes.DESTROY_MATCH_CALLBACK);
        }
        // NOTE: Lots of problems with this callback not being called.
        // Somehow it is solved after I started using matchMaker.SetMatchAttributes.
    }



}
