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
/// the high level API functions which allow hosting and joining rooms. Adding
/// on to these features, I have written code to make the hosting and joining
/// process more foolproof.
/// 
/// Some problems with using the NetworkManager class without modifications:
///     Rapidly creating and joining matches messes with the callbacks and MatchMaker modes.
///     Matches are not unlisted when the match's game has begun, but they should be.
///     Destroying a match while somebody is joining causes the joiner to be stuck in-between menus.
/// 
/// auth Wesley Wu
/// </summary>
public class Server : NetworkManager
{
    /// <summary>
    /// The domain we use to check for match listings
    /// </summary>
    private const int CHECKING_DOMAIN = 0;

    public bool LAN_mode = false;

    private IEnumerator co = null;

    private void Start()
    {
        StartMatchMaker();
    }

    private void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        // Do not let the application quit if the game is waiting for something.
        if (!Settings.WAIT_FOR.Contains(Settings.WaitTypes.QUIT_GAME) && Settings.WAIT_FOR.Count > 0)
        {
            Application.CancelQuit();
            return;
        }
        if (!Settings.WAIT_FOR.Contains(Settings.WaitTypes.QUIT_GAME))
        {
            Application.CancelQuit();
            Debug.LogError("OnApplicationQuit()");
            Settings.WAIT_FOR.Add(Settings.WaitTypes.QUIT_GAME);

            // In the case that you're the creator of a game, you must destroy the match before quitting.
            if (Player.mine != null && Player.mine.is_host)
            {
                if (LAN_mode)
                    Application.Quit();
                DestroyInternetMatch();
            }
            // Reset to home to remove all existing network dependencies before quitting.
            else
            {
                ResetToHome();
            }
        }
    }
    


    private IEnumerator EnsureJoinedMatchExists(MatchInfoSnapshot match_snapshot)
    {
        while (true)
        {
            ListInternetMatches();
            if (match_snapshot != null && matches != null)
            {
                bool match_exists = false;
                foreach (MatchInfoSnapshot mis in matches)
                {
                    if (mis.name == match_snapshot.name)
                        match_exists = true;
                }
                if (!match_exists)
                {
                    ResetToHome();
                    break;
                }
            }
            yield return null;
        }
    }


    /// <summary>
    /// Flush all waits, renew Host/MatchMaker, and send user to home menu.
    /// </summary>
    public void ResetToHome()
    {
        Debug.Log(Settings.WAIT_FOR.Contains(Settings.WaitTypes.QUIT_GAME));
        if (Settings.WAIT_FOR.Contains(Settings.WaitTypes.QUIT_GAME))
        {
            Application.Quit();
        }
        else
        {
            Settings.WAIT_FOR = new List<Settings.WaitTypes>();
            StopHost();
            StopMatchMaker();
            StartMatchMaker();
            MenuManager.current_menu = typeof(MenuPreGameHome);
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
        matchMaker.ListMatches(0, 5, "", true, 0, CHECKING_DOMAIN, OnMatchList);
    }

    public void JoinInternetMatch(MatchInfoSnapshot mis)
    {
        matchMaker.JoinMatch(mis.networkId, "", "", "", 0, CHECKING_DOMAIN, OnMatchJoined);
        co = EnsureJoinedMatchExists(mis);
        StartCoroutine(co);
    }

    public void DestroyInternetMatch()
    {
        if (LAN_mode)
            return;
        UnlistMatch();
        matchMaker.DestroyMatch(matchInfo.networkId, matchInfo.domain, OnDestroyMatch);
    }

    
    public void UnlistMatch()
    {
        if (LAN_mode)
            return;
        matchMaker.SetMatchAttributes(this.matchInfo.networkId, false, this.matchInfo.domain, OnSetMatchAttributes);
    }


    // ------------------------------------------------- UNET Overrides -------------------------------------------------
    

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.LogError("OnClientError" + System.DateTime.Now.TimeOfDay);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.LogError("OnClientDisconnect" + System.DateTime.Now.TimeOfDay);
        ResetToHome();
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        base.OnClientNotReady(conn);
        Debug.LogError("OnClientNotReady" + System.DateTime.Now.TimeOfDay);
    }

    

    

    

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.LogError("OnServerDisconnect" + System.DateTime.Now.TimeOfDay);
        // Somebody exited the game
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);
        Debug.LogError("OnServerError" + System.DateTime.Now.TimeOfDay);
    }

    

    public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        Debug.LogError("OnServerRemovePlayer" + System.DateTime.Now.TimeOfDay);
    }
    

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
        Debug.LogError("OnMatchJoined" + success + System.DateTime.Now.TimeOfDay);
        if (this.matchInfo == null || this.matchInfo.nodeId == NodeID.Invalid)
        {
            ResetToHome();
        }
    }


    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        base.OnMatchList(success, extendedInfo, matchList);
        //Debug.LogError("OnMatchList" + System.DateTime.Now.TimeOfDay);
    }






    // ------------------------------------------------- Network Flow -------------------------------------------------


    /// <summary>
    /// This hook is called when a host is started.
    /// StartHost has multiple signatures, but they all cause this hook to be called.
    /// </summary>
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.LogError("OnStartHost" + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// This hook is called when a server is started - including when a host is started.
    /// StartServer has multiple signatures, but they all cause this hook to be called.
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.LogError("OnStartServer" + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// Called on the server when a new client connects.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.LogError("OnServerConnect" + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// This is a hook that is invoked when the client is started.
    /// StartClient has multiple signatures, but they all cause this hook to be called.
    /// </summary>
    /// <param name="client"></param>
    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        Debug.LogError("OnStartClient" + System.DateTime.Now.TimeOfDay);
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
        Debug.LogError("OnMatchCreate: success=" + success + System.DateTime.Now.TimeOfDay);
        if (success)
        {
            this.matchInfo = matchInfo;
            Settings.WAIT_FOR.Remove(Settings.WaitTypes.CREATE_MATCH_CALLBACK);
            Settings.WAIT_FOR.Add(Settings.WaitTypes.CREATE_MATCH);
        }
        else
        {
            ResetToHome();
        }
    }

    /// <summary>
    /// Called on the client when connected to a server.
    /// The default implementation of this function sets the client as ready and adds a player.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.LogError("OnClientConnect" + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName"></param>
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Debug.LogError("OnServerSceneChanged" + System.DateTime.Now.TimeOfDay);
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
        Debug.LogError("OnServerReady" + System.DateTime.Now.TimeOfDay);
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
        Debug.LogError("OnServerAddPlayer" + System.DateTime.Now.TimeOfDay);
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
        Debug.LogError("OnClientSceneChanged" + System.DateTime.Now.TimeOfDay);
    }

    private IEnumerator WaitForPlayer()
    {
        while (Player.mine == null)
            yield return null;
        // At this point, we've finished loading everything we need to, and if there were errors in match
        // creation, we will need to do some cleanup

        if (Player.mine.is_host)
        {
            Settings.WAIT_FOR.Remove(Settings.WaitTypes.CREATE_MATCH);
        }
        else
        {
            Settings.WAIT_FOR.Remove(Settings.WaitTypes.JOIN_MATCH);
            StopCoroutine(co);
            if (this.matchInfo == null || this.matchInfo.nodeId == NodeID.Invalid)
                ResetToHome();
        }
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
        Debug.LogError("OnSetMatchAttributes callback: success=" + success + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// This hook is called when a host is stopped.
    /// </summary>
    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.LogError("OnStopHost" + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// This hook is called when a server is stopped - including when a host is stopped.
    /// </summary>
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.LogError("OnStopServer" + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// This hook is called when a client is stopped.
    /// </summary>
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.LogError("OnStopClient" + System.DateTime.Now.TimeOfDay);
    }

    /// <summary>
    /// Callback that happens when a NetworkMatch.DestroyMatch request has been processed on the server.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        base.OnDestroyMatch(success, extendedInfo);
        Debug.LogError("OnDestroyMatch: success=" + success + System.DateTime.Now.TimeOfDay);
        Debug.Log(Settings.WAIT_FOR.Contains(Settings.WaitTypes.QUIT_GAME));
        if (Settings.WAIT_FOR.Contains(Settings.WaitTypes.QUIT_GAME))
            Application.Quit();
        if (success)
        {
            ResetToHome();
        }
        // NOTE: Lots of problems with this callback not being called.
        // Somehow it is solved after I started using matchMaker.SetMatchAttributes.
    }



}
