namespace Database
{
    using System.Collections;
    using UnityEngine;
    using MySQL;
    using Mediator;

    public sealed class DataSystem : SystemBase
    {
        public SystemObject systemObject;

        public Query query;

        public Profile profile;
        public Friend friend;
        public FriendRequest friendRequest;
        public Party party;
        public PartyInvitation partyInvitation;

        public bool loggedIn = false;

        public DataSystem(SystemObject systemObject)
        {
            this.systemObject = systemObject;

            query = new Query();

            profile = new Profile(this);
            friend = new Friend(this);
            friendRequest = new FriendRequest(this);
            party = new Party(this);
            partyInvitation = new PartyInvitation(this);
        }

        public override void Update() { }
        public override void OnGUI() { }

        public void Login(string name)
        {
            profile.userName = name;
            loggedIn = true;
        }

        public Coroutine CoroutineStart(IEnumerator routine)
        {
            return systemObject.CoroutineStart(routine);
        }

        public void CoroutineStop(Coroutine coroutine)
        {
            systemObject.CoroutineStop(coroutine);
        }
    }
}