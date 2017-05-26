namespace Data.Network
{
    using UnityEngine.Networking;
    using UnityEngine.Networking.Types;

    public class Migration : NATTraversal.MigrationManager
    {
        private Database.DatabaseSystem databaseSystem;
        private bool leave = true;
        private Database.MySQL.Query query;
        private Database.Mediator.Profile profile;

        private void Awake()
        {
            databaseSystem = FindObjectOfType<Database.DatabaseSystem>();
            query = databaseSystem.query;
            profile = databaseSystem.profile;
        }

        private void Update()
        {
            if (hostWasShutdown)
            {
                DisconnectOnHost();
            }
            else if (disconnectedFromHost && oldServerConnectionId != -1)
            {
                DisconnectOnClient();
            }
        }

        private void DisconnectOnHost()
        {
            if (leave) // We want to leave for good
            {
                networkManager.SetupMigrationManager(null);
                //networkManager.StopHost(); // We already stopped host.
                Reset(ClientScene.ReconnectIdInvalid);
            }
            else // We want to reconnect
            {
                bool youAreNewHost;
                if (FindNewHost(out newHost, out youAreNewHost)) // Check who is the new host
                {
                    newHostAddress = newHost.address;
                    if (!youAreNewHost) // You're not the host, so reconnect
                    {
                        waitingReconnectToNewHost = true;
                        Reset(ClientScene.ReconnectIdHost);
                        networkManager.networkAddress = newHostAddress;
                        networkManager.StartClientAll(newHost.address, newHost.internalIP, newHost.guid, NetworkID.Invalid, newHost.externalIPv6, newHost.internalIPv6);
                    }
                }
            }
        }

        private void DisconnectOnClient()
        {
            bool youAreNewHost;
            if (FindNewHost(out newHost, out youAreNewHost)) // Check who is the new host
            {
                newHostAddress = newHost.address;
                if (youAreNewHost) // If you become host
                {
                    waitingToBecomeNewHost = true;
                    NetworkServer.Configure(networkManager.topo);
                    BecomeNewHost(networkManager.networkPort);
                    StartCoroutine(query.Match.Add(profile.userName, newHost.address, newHost.internalIP, newHost.externalIPv6, newHost.internalIPv6, newHost.guid.ToString()));
                }
                else // If you don't become host
                {
                    waitingReconnectToNewHost = true;
                    ReconnectToNewHost();
                }
            }
        }
    }
}