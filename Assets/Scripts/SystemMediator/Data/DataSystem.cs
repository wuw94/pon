namespace Data
{
    using UnityEngine;

    public sealed class DataSystem : MonoBehaviour
    {
        public SystemMediator systemMediator;
        public Database.DatabaseSystem databaseSystem;
        public Network.NetworkSystem networkSystem;
    }
}