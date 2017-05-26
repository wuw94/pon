namespace Data.Network
{
    using System.Collections;
    using UnityEngine;

    public class NetworkSystem : MonoBehaviour
    {
        public DataSystem dataSystem;
        [SerializeField] private NetworkScript networkManager;
        public UI.Menu DisconnectedMenu;
        public Coroutine ensureMatchExistsCoroutine;

        public void StartModule()
        {
            networkManager.StartModule();
        }

        public void StopModule()
        {
            networkManager.expectedCallbacks.Clear();
            networkManager.StopModule();
        }

        public void Host()
        {
            networkManager.Host();
        }
    
        public void Join(Database.Mediator.Match.MatchInfo matchInfo)
        {
            networkManager.Join(matchInfo);
        }

        public void EnsureJoinedMatchExists(Database.Mediator.Match.MatchInfo matchInfo)
        {
            ensureMatchExistsCoroutine = StartCoroutine(EnsureJoinedMatchExistsRoutine(matchInfo));
        }

        public void EnsureJoinedMatchExistsStop()
        {
            if (ensureMatchExistsCoroutine != null) StopCoroutine(ensureMatchExistsCoroutine);
        }

        private IEnumerator EnsureJoinedMatchExistsRoutine(Data.Database.Mediator.Match.MatchInfo matchInfo)
        {
            Database.Mediator.Match match = dataSystem.databaseSystem.match;
            NetworkSystem networkSystem = dataSystem.networkSystem;
            while (true)
            {
                if (!match.Exists(matchInfo))
                {
                    networkSystem.StopModule();
                    dataSystem.systemMediator.uiSystem.menuSystem.BackUntil(DisconnectedMenu);
                    break;
                }
                yield return null;
            }
        }

        public bool Ready()
        {
            return networkManager.Ready();
        }
    }
}