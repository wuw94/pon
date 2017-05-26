namespace Data.Network
{
    using System.Collections.Generic;

    public partial class NetworkScript
    {
        public enum ExpectedCallback
        {
            OnClientConnect,
            OnClientDisconnect,
            OnClientError,
            OnClientNotReady,
            OnClientSceneChanged,
            OnConnectionInfoConfirmationReceivedOnClient,
            OnConnectionReplacedClient,
            OnConnectionReplacedServer,
            OnDestroyMatch,
            OnDoneConnectingToFacilitator,
            OnDropConnection,
            OnHolePunchedClient,
            OnHolePunchedServer,
            OnMatchCreate,
            OnMatchDestroyed,
            OnMatchDropped,
            OnMatchList,
            OnMatchJoined,
            OnMultiClientConnect,
            OnMultiClientConnectInternal,
            OnMultiClientDisconnect,
            OnServerAddPlayer,
            OnServerConnect,
            OnServerDisconnect,
            OnServerError,
            OnServerReady,
            OnServerRemovePlayer,
            OnServerSceneChanged,
            OnSetConnectionInfo,
            OnSetMatchAttributes,
            OnStartClient,
            OnStartHost,
            OnStartServer,
            OnStopClient,
            OnStopHost,
            OnStopServer
        };

        private ExpectedCallback[] callbacksHost = new ExpectedCallback[]
        {
            ExpectedCallback.OnStartHost,
            ExpectedCallback.OnStartServer,
            ExpectedCallback.OnServerConnect,
            ExpectedCallback.OnClientConnect,
            ExpectedCallback.OnServerSceneChanged,
            ExpectedCallback.OnServerReady,
            ExpectedCallback.OnServerAddPlayer,
            ExpectedCallback.OnClientSceneChanged,
            ExpectedCallback.OnMatchCreate
        };

        private ExpectedCallback[] callbacksJoin = new ExpectedCallback[]
        {
            ExpectedCallback.OnConnectionInfoConfirmationReceivedOnClient,
            ExpectedCallback.OnClientConnect,
            ExpectedCallback.OnMultiClientConnect,
            ExpectedCallback.OnClientSceneChanged
            //ExpectedCallback.OnMatchJoined
        };

        private ExpectedCallback[] callbacksHostStop = new ExpectedCallback[]
        {
            ExpectedCallback.OnClientNotReady,
            ExpectedCallback.OnStopHost,
            ExpectedCallback.OnStopServer,
            ExpectedCallback.OnStopClient
        };

        private ExpectedCallback[] callbacksClientStop = new ExpectedCallback[]
        {
            ExpectedCallback.OnStopClient
        };

        public HashSet<ExpectedCallback> expectedCallbacks = new HashSet<ExpectedCallback>();
    
        public void CallbacksAdd(ExpectedCallback[] callbacks)
        {
            for (int i = 0; i < callbacks.Length; i++)
                expectedCallbacks.Add(callbacks[i]);
        }

        /// <summary>
        /// If the NetworkModule is waiting on a callback, it is not ready.
        /// </summary>
        public bool Ready()
        {
            return expectedCallbacks.Count == 0;
        }
    }
}