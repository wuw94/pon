namespace Data.Network
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Types;

    public partial class NetworkScript : NATTraversal.NetworkManager
    {
        public Database.DatabaseSystem databaseSystem;
        public UI.MenuSystem menuSystem;
        private Database.MySQL.Query query;
        private Database.Mediator.Profile profile;
        private Database.Mediator.Match match;
    
        public override void Awake()
        {
            base.Awake();

            query = databaseSystem.query;
            profile = databaseSystem.profile;
            match = databaseSystem.match;
        }

        public void OnGUI()
        {
            GUI.Label(new Rect(0, 0, Screen.width, 20), "Callbacks: " + expectedCallbacks.Count);
        }

        public void StartModule()
        {
            StartCoroutine(StartModuleRoutine());
        }

        public void StopModule()
        {
            natHelper.DisconnectFromFacilitator();
            if (NetworkServer.active)
            {
                CallbacksAdd(callbacksHostStop);
                NetworkServer.SetAllClientsNotReady();
                StopHost();
            }
            else
            {
                CallbacksAdd(callbacksClientStop);
                StopClient();
            }
        }

        public void Host()
        {
            StartCoroutine(HostRoutine());
        }

        public void Join(Database.Mediator.Match.MatchInfo matchInfo)
        {
            if (matchInfo == Database.Mediator.Match.MatchInfo.Invalid)
            {
                Debug.LogError("Match to-join is invalid and therefore canceled");
                return;
            }
            CallbacksAdd(callbacksJoin);
            StartClientAll(matchInfo.externalIP, matchInfo.internalIP, matchInfo.guid, NetworkID.Invalid, matchInfo.externalIPv6, matchInfo.internalIPv6);
        }

        private IEnumerator StartModuleRoutine()
        {
            natHelper.DisconnectFromFacilitator();
            expectedCallbacks.Add(ExpectedCallback.OnDoneConnectingToFacilitator);
            yield return natHelper.connectToNATFacilitator();
            StartMatchMaker();
        }

        private IEnumerator HostRoutine()
        {
            yield return StartModuleRoutine();
            CallbacksAdd(callbacksHost);
            while (expectedCallbacks.Contains(ExpectedCallback.OnDoneConnectingToFacilitator))
                yield return null;
            StartHostAll(databaseSystem.profile.userName, matchSize);
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
    }
}