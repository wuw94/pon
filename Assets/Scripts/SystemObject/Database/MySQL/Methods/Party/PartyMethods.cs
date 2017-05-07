namespace Database.MySQL
{
    using System.Collections;

    public interface IPartyMethods
    {
        IEnumerator List(Table output, string name);
        IEnumerator ChangeLeaderParty(string old_leader, string new_leader);
        IEnumerator ChangeLeaderSingle(string name, string new_leader);
        IEnumerator Join(string name, string other);
    }

    public partial class Query : IPartyMethods
    {
        public IPartyMethods Party { get { return this as IPartyMethods; } }

        /// <summary>
        /// List people in my party.
        /// Accessors ["name"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        IEnumerator IPartyMethods.List(Table output, string name)
        {
            yield return Run(output, "PartyList(name)", "name", name);
        }

        /// <summary>
        /// Change the party leader for everybody in a party.
        /// </summary>
        /// <param name="old_leader"></param>
        /// <param name="new_leader"></param>
        /// <returns></returns>
        IEnumerator IPartyMethods.ChangeLeaderParty(string old_leader, string new_leader)
        {
            yield return Party.ChangeLeaderSingle(new_leader, new_leader);
            yield return Run("PartyChangeLeaderParty(old,new)", "old", old_leader, "new", new_leader);
        }

        /// <summary>
        /// Change the party leader for a single person.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        IEnumerator IPartyMethods.ChangeLeaderSingle(string name, string new_leader)
        {
            yield return Run("PartyChangeLeaderSingle(name,leader)", "name", name, "leader", new_leader);
        }

        /// <summary>
        /// Join another person's party.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        IEnumerator IPartyMethods.Join(string name, string other)
        {
            yield return Run("PartyJoin(name,other)", "name", name, "other", other);
        }
    }
}