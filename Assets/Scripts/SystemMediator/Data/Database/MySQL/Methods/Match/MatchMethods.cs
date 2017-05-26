namespace Data.Database.MySQL
{
    using System.Collections;

    public interface IMatchMethods
    {
        IEnumerator List(Table output);
        IEnumerator Add(string name, string externalIP, string internalIP, string externalIPv6, string internalIPv6, string guid);
        IEnumerator Remove(string name);
    }

    public partial class Query : IMatchMethods
    {
        public IMatchMethods Match { get { return this as IMatchMethods; } }

        /// <summary>
        /// List all matches
        /// Accessors ["name"] ["external_ip"] ["internal_ip"] ["external_ipv6"] ["internal_ipv6"]
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        IEnumerator IMatchMethods.List(Table output)
        {
            yield return Run(output, "MatchList()");
        }
        
        /// <summary>
        /// Add a match.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="externalIP"></param>
        /// <param name="internalIP"></param>
        /// <param name="externalIPv6"></param>
        /// <param name="internalIPv6"></param>
        /// <returns></returns>
        IEnumerator IMatchMethods.Add(string name, string externalIP, string internalIP, string externalIPv6, string internalIPv6, string guid)
        {
            yield return Run("MatchAdd(6)", "name", name, "externalIP", externalIP, "internalIP", internalIP, "externalIPv6", externalIPv6, "internalIPv6", internalIPv6, "guid", guid);
        }

        /// <summary>
        /// Remove a match.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerator IMatchMethods.Remove(string name)
        {
            yield return Run("MatchRemove(name)", "name", name);
        }
    }
}