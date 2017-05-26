namespace Data.Database.Mediator
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
        
        public Friend(DatabaseSystem databaseSystem) : base(databaseSystem)
        {
            wait = 0.5f;
        }
        
        protected override IEnumerator UpdateTables()
        {
            yield return base.UpdateTables();

            /*
            if (databaseSystem.loggedIn && databaseSystem.dataSystem.systemObject.uiSystem.menuSystem.current == typeof(UI.FriendsMenu))
            {
                yield return query.Friend.ListOnline(onlineTable, databaseSystem.profile.userName);
                online = onlineTable[FIELD_NAME];
                
                yield return query.Friend.ListOffline(offlineTable, databaseSystem.profile.userName);
                offline = offlineTable[FIELD_NAME];
            }
            */
            yield return null;
        }
    }
}