namespace Data.Database.Mediator
{
    using System.Collections;

    public sealed class Party : DataMediator
    {
        private MySQL.Table table = new MySQL.Table();
        public const int MAX_SIZE = 4;

        public Party(DatabaseSystem dataSystem) : base(dataSystem)
        {
            wait = 0.1f;
        }

        protected override IEnumerator UpdateTables()
        {
            yield return base.UpdateTables();

            if (databaseSystem.loggedIn)
            {
                yield return query.Party.List(table, databaseSystem.profile.userName);
            }
            yield return null;
        }

        /// <summary>
        /// Number of people in my party, not including me
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return table.Length;
        }
        
        /// <summary>
        /// Returns the name of person in my party, given index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string Name(int i)
        {
            return table["name"][i];
        }
        /// <summary>
        /// Returns the leader of my party.
        /// </summary>
        /// <returns></returns>
        public string Leader()
        {
            if (table.Length == 0)
                return databaseSystem.profile.userName;
            return table["leader"][0];
        }
        /// <summary>
        /// Returns true if you are the party leader
        /// </summary>
        /// <returns></returns>
        public bool IsLeader(string name)
        {
            return name == Leader();
        }
    }
}