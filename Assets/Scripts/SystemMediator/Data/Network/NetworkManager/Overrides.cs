namespace Data.Network
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;

    public partial class NetworkScript : NATTraversal.NetworkManager
    {
        private bool showLogError = true;

    
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnClientConnect);
            if (showLogError) Debug.LogError("OnClientConnect()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnClientDisconnect);
            if (showLogError) Debug.LogError("OnClientDisconnect()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            base.OnClientError(conn, errorCode);
            expectedCallbacks.Remove(ExpectedCallback.OnClientError);
            if (showLogError) Debug.LogError("OnClientError()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnClientNotReady(NetworkConnection conn)
        {
            base.OnClientNotReady(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnClientNotReady);
            if (showLogError) Debug.LogError("OnClientNotReady()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnClientSceneChanged);
            if (showLogError) Debug.LogError("OnClientSceneChanged()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnConnectionInfoConfirmationReceivedOnClient(NetworkMessage msg)
        {
            base.OnConnectionInfoConfirmationReceivedOnClient(msg);
            expectedCallbacks.Remove(ExpectedCallback.OnConnectionInfoConfirmationReceivedOnClient);
            if (showLogError) Debug.LogError("OnConnectionInfoConfirmationReceivedOnClient()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnConnectionReplacedClient(NetworkConnection oldConnection, NetworkConnection newConnection)
        {
            base.OnConnectionReplacedClient(oldConnection, newConnection);
            expectedCallbacks.Remove(ExpectedCallback.OnConnectionReplacedClient);
            if (showLogError) Debug.LogError("OnConnectionReplacedClient()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnConnectionReplacedServer(NetworkConnection oldConnection, NetworkConnection newConnection)
        {
            base.OnConnectionReplacedServer(oldConnection, newConnection);
            expectedCallbacks.Remove(ExpectedCallback.OnConnectionReplacedServer);
            if (showLogError) Debug.LogError("OnConnectionReplacedServer()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnDestroyMatch(bool success, string extendedInfo)
        {
            base.OnDestroyMatch(success, extendedInfo);
            expectedCallbacks.Remove(ExpectedCallback.OnDestroyMatch);
            if (showLogError) Debug.LogError("OnDestroyMatch()" + success + System.DateTime.Now.TimeOfDay);
        }

        public override void OnDoneConnectingToFacilitator(ulong guid)
        {
            base.OnDoneConnectingToFacilitator(guid);
            expectedCallbacks.Remove(ExpectedCallback.OnDoneConnectingToFacilitator);
            if (showLogError) Debug.LogError("OnDoneConnectingToFacilitator()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnDropConnection(bool success, string extendedInfo)
        {
            base.OnDropConnection(success, extendedInfo);
            expectedCallbacks.Remove(ExpectedCallback.OnDropConnection);
            if (showLogError) Debug.LogError("OnDropConnection()" + success + System.DateTime.Now.TimeOfDay);
        }

        public override void OnHolePunchedClient(int natListenPort, int natConnectPort, bool success)
        {
            base.OnHolePunchedClient(natListenPort, natConnectPort, success);
            expectedCallbacks.Remove(ExpectedCallback.OnHolePunchedClient);
            if (showLogError) Debug.LogError("OnHolePunchedClient()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnHolePunchedServer(int natListenPort, ulong clientGUID)
        {
            base.OnHolePunchedServer(natListenPort, clientGUID);
            expectedCallbacks.Remove(ExpectedCallback.OnHolePunchedServer);
            if (showLogError) Debug.LogError("OnHolePunchedServer()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            expectedCallbacks.Remove(ExpectedCallback.OnMatchCreate);
            if (showLogError) Debug.LogError("OnMatchCreate()" + success + System.DateTime.Now.TimeOfDay);
            StartCoroutine(query.Match.Add(profile.userName, externalIP, hostInternalIP, externalIPv6, hostInternalIPv6, natHelper.guid.ToString()));
        }

        public override void OnMatchDestroyed(bool success, string extendedInfo)
        {
            base.OnMatchDestroyed(success, extendedInfo);
            expectedCallbacks.Remove(ExpectedCallback.OnMatchDestroyed);
            if (showLogError) Debug.LogError("OnMatchDestroyed()" + success + System.DateTime.Now.TimeOfDay);
        }

        public override void OnMatchDropped(bool success, string extendedInfo)
        {
            base.OnMatchDropped(success, extendedInfo);
            expectedCallbacks.Remove(ExpectedCallback.OnMatchDropped);
            if (showLogError) Debug.LogError("OnMatchDropped()" + success + System.DateTime.Now.TimeOfDay);
        }

        public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
        {
            base.OnMatchList(success, extendedInfo, matchList);
            expectedCallbacks.Remove(ExpectedCallback.OnMatchList);
            //if (showLogError) Debug.LogError("OnMatchList()" + success + System.DateTime.Now.TimeOfDay);
        }

        public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchJoined(success, extendedInfo, matchInfo);
            expectedCallbacks.Remove(ExpectedCallback.OnMatchJoined);
            if (showLogError) Debug.LogError("OnMatchJoined()" + success + System.DateTime.Now.TimeOfDay);
        }


        public override void OnMultiClientConnect(NetworkConnection conn)
        {
            base.OnMultiClientConnect(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnMultiClientConnect);
            if (showLogError) Debug.LogError("OnMultiClientConnect()" + System.DateTime.Now.TimeOfDay);
            FindObjectOfType<NetworkSystem>().EnsureJoinedMatchExistsStop();
        }

        protected override void OnMultiClientConnectInternal(NetworkConnection conn)
        {
            base.OnMultiClientConnectInternal(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnMultiClientConnectInternal);
            if (showLogError) Debug.LogError("OnMultiClientConnectInternal()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnMultiClientDisconnect(NetworkConnection conn)
        {
            base.OnMultiClientDisconnect(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnMultiClientDisconnect);
            if (showLogError) Debug.LogError("OnMultiClientDisconnect()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
            expectedCallbacks.Remove(ExpectedCallback.OnServerAddPlayer);
            if (showLogError) Debug.LogError("OnServerAddPlayer()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
            expectedCallbacks.Remove(ExpectedCallback.OnServerAddPlayer);
            if (showLogError) Debug.LogError("OnServerAddPlayer(+extraMessageReader)" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnServerConnect);
            if (showLogError) Debug.LogError("OnServerConnect()" + System.DateTime.Now.TimeOfDay);
            expectedCallbacks.Add(ExpectedCallback.OnServerReady);
            expectedCallbacks.Add(ExpectedCallback.OnServerAddPlayer);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnServerDisconnect);
            if (showLogError) Debug.LogError("OnServerDisconnect()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnServerError(NetworkConnection conn, int errorCode)
        {
            base.OnServerError(conn, errorCode);
            expectedCallbacks.Remove(ExpectedCallback.OnServerError);
            if (showLogError) Debug.LogError("OnServerError()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            expectedCallbacks.Remove(ExpectedCallback.OnServerReady);
            if (showLogError) Debug.LogError("OnServerReady()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            base.OnServerRemovePlayer(conn, player);
            expectedCallbacks.Remove(ExpectedCallback.OnServerRemovePlayer);
            if (showLogError) Debug.LogError("OnServerRemovePlayer()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            expectedCallbacks.Remove(ExpectedCallback.OnServerSceneChanged);
            if (showLogError) Debug.LogError("OnServerSceneChanged()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnSetConnectionInfo(NetworkMessage msg)
        {
            base.OnSetConnectionInfo(msg);
            expectedCallbacks.Remove(ExpectedCallback.OnSetConnectionInfo);
            if (showLogError) Debug.LogError("OnSetConnectionInfo()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnSetMatchAttributes(bool success, string extendedInfo)
        {
            base.OnSetMatchAttributes(success, extendedInfo);
            expectedCallbacks.Remove(ExpectedCallback.OnSetMatchAttributes);
            if (showLogError) Debug.LogError("OnSetMatchAttributes()" + success + System.DateTime.Now.TimeOfDay);
        }

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
            expectedCallbacks.Remove(ExpectedCallback.OnStartClient);
            if (showLogError) Debug.LogError("OnStartClient()" + System.DateTime.Now.TimeOfDay);
            RegisterPrefabs();
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            expectedCallbacks.Remove(ExpectedCallback.OnStartHost);
            if (showLogError) Debug.LogError("OnStartHost()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            expectedCallbacks.Remove(ExpectedCallback.OnStartServer);
            if (showLogError) Debug.LogError("OnStartServer()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            expectedCallbacks.Remove(ExpectedCallback.OnStopClient);
            if (showLogError) Debug.LogError("OnStopClient()" + System.DateTime.Now.TimeOfDay);
        }

        public override void OnStopHost()
        {
            base.OnStopHost();
            expectedCallbacks.Remove(ExpectedCallback.OnStopHost);
            if (showLogError) Debug.LogError("OnStopHost()" + System.DateTime.Now.TimeOfDay);
            StartCoroutine(query.Match.Remove(profile.userName));
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            expectedCallbacks.Remove(ExpectedCallback.OnStopServer);
            if (showLogError) Debug.LogError("OnStopServer()" + System.DateTime.Now.TimeOfDay);
        }
    }
}