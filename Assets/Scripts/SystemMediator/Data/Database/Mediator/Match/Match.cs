namespace Data.Database.Mediator
{
    using System.Collections;

    public sealed class Match : DataMediator
    {
        public struct MatchInfo
        {
            public string name;
            public ulong guid;
            public string externalIP;
            public string internalIP;
            public string externalIPv6;
            public string internalIPv6;

            public static MatchInfo Invalid { get { return new MatchInfo(); } }

            public static bool operator ==(MatchInfo first, MatchInfo second)
            {
                return first.name == second.name && first.externalIP == second.externalIP && first.guid == second.guid;
            }

            public static bool operator !=(MatchInfo first, MatchInfo second)
            {
                return !(first == second);
            }

            public override string ToString()
            {
                string to_return = "MatchInfo:";
                to_return += "\n name = " + name;
                to_return += "\n guid = " + guid;
                to_return += "\n externalIP = " + externalIP;
                to_return += "\n internalIP = " + internalIP;
                to_return += "\n externalIPv6 = " + externalIPv6;
                to_return += "\n internalIPv6 = " + internalIPv6;
                return to_return;
            }
        }

        private MySQL.Table table = new MySQL.Table();

        public Match(DatabaseSystem dataSystem) : base(dataSystem)
        {
            wait = 0.1f;
        }

        protected override IEnumerator UpdateTables()
        {
            yield return base.UpdateTables();

            if (databaseSystem.loggedIn)
            {
                yield return query.Match.List(table);
            }
            yield return null;
        }

        public bool Exists(MatchInfo matchInfo)
        {
            for (int i = 0; i < table.Length; i++)
                if (this[i] == matchInfo)
                    return true;
            return false;
        }

        public int Count()
        {
            return table.Length;
        }

        public MatchInfo this[int index]
        {
            get
            {
                MatchInfo matchInfo = new MatchInfo();
                matchInfo.name = table["name"][index];
                matchInfo.guid = System.Convert.ToUInt64(table["guid"][index], 16);
                matchInfo.externalIP = table["externalIP"][index];
                matchInfo.internalIP = table["internalIP"][index];
                matchInfo.externalIPv6 = table["externalIPv6"][index];
                matchInfo.internalIPv6 = table["internalIPv6"][index];
                return matchInfo;
            }
        }
    }
}