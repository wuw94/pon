namespace Database.Mediator
{
    using System.Collections;
    using MySQL;

    public class Friend : DataMediator
    {
        Table onlineTable = new Table();
        Table offlineTable = new Table();
        private const string FIELD_NAME = "friend";
        public Table.Field online = new Table.Field();
        public Table.Field offline = new Table.Field();
        
        public Friend(DataSystem dataSystem) : base(dataSystem)
        {
            wait = 0.5f;
        }
        
        protected override IEnumerator UpdateTables()
        {
            yield return base.UpdateTables();

            if (dataSystem.loggedIn && dataSystem.systemObject.uiSystem.menuSystem.current == typeof(UI.FriendsMenu))
            {
                yield return query.Friend.ListOnline(onlineTable, dataSystem.profile.userName);
                online = onlineTable[FIELD_NAME];
                
                yield return query.Friend.ListOffline(offlineTable, dataSystem.profile.userName);
                offline = offlineTable[FIELD_NAME];
            }
            yield return null;
        }
    }
}