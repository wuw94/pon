namespace Data.Database
{
    using System.Collections;
    using UnityEngine;
    using MySQL;
    using Mediator;

    public sealed class DatabaseSystem : MonoBehaviour
    {
        public DataSystem dataSystem;
        public Query query;

        public Profile profile;
        public Friend friend;
        public FriendRequest friendRequest;
        public Match match;
        public Party party;
        public PartyInvitation partyInvitation;

        public bool loggedIn = false;

        private void Awake()
        {
            query = new Query();

            profile = new Profile(this);
            friend = new Friend(this);
            friendRequest = new FriendRequest(this);
            match = new Match(this);
            party = new Party(this);
            partyInvitation = new PartyInvitation(this);
        }

        public void Login(string name)
        {
            profile.userName = name;
            loggedIn = true;
        }

        public Coroutine CoroutineStart(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void CoroutineStop(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
    }
}