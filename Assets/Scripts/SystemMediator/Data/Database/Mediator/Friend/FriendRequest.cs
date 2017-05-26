namespace Data.Database.Mediator
{
    using System.Collections;
    using MySQL;

    public class FriendRequest : DataMediator
    {
        private const string FIELD_NAME = "name";
        public Table.Field receiver = new Table.Field();
        public Table.Field requester = new Table.Field();

        public FriendRequest(DatabaseSystem databaseSystem) : base(databaseSystem)
        {
            wait = 1;
        }

        protected override IEnumerator UpdateTables()
        {
            yield return base.UpdateTables();

            /*
            if (databaseSystem.loggedIn && databaseSystem.dataSystem.systemObject.uiSystem.menuSystem.current == typeof(UI.FriendsMenu))
            {
                Table receiverTable = new Table();
                yield return query.FriendRequest.ListReceiver(receiverTable, databaseSystem.profile.userName);
                receiver = receiverTable[FIELD_NAME];

                Table requesterTable = new Table();
                yield return query.FriendRequest.ListRequester(requesterTable, databaseSystem.profile.userName);
                requester = requesterTable[FIELD_NAME];
            }
            */
            yield return null;
        }
    }
}