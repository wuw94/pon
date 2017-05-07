namespace Database.Mediator
{
    using System.Collections;
    using MySQL;

    public class FriendRequest : DataMediator
    {
        private const string FIELD_NAME = "name";
        public Table.Field receiver = new Table.Field();
        public Table.Field requester = new Table.Field();

        public FriendRequest(DataSystem dataSystem) : base(dataSystem)
        {
            wait = 1;
        }

        protected override IEnumerator UpdateTables()
        {
            yield return base.UpdateTables();

            if (dataSystem.loggedIn && dataSystem.systemObject.uiSystem.menuSystem.current == typeof(UI.FriendsMenu))
            {
                Table receiverTable = new Table();
                yield return query.FriendRequest.ListReceiver(receiverTable, dataSystem.profile.userName);
                receiver = receiverTable[FIELD_NAME];

                Table requesterTable = new Table();
                yield return query.FriendRequest.ListRequester(requesterTable, dataSystem.profile.userName);
                requester = requesterTable[FIELD_NAME];
            }
            yield return null;
        }
    }
}