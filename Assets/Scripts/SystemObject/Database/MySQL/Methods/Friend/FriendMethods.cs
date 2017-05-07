namespace Database.MySQL
{
    using System.Collections;

    public interface IFriendMethods
    {
        IEnumerator List(Table output, string myname);
        IEnumerator ListOnline(Table output, string myname);
        IEnumerator ListOffline(Table output, string myname);
        IEnumerator Delete(string myname, string other);
    }

    public partial class Query : IFriendMethods
    {
        public IFriendMethods Friend { get { return this as IFriendMethods; } }

        /// <summary>
        /// List my friends by name, alphabetically
        /// Accessors ["friend"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="myname"></param>
        /// <returns></returns>
        IEnumerator IFriendMethods.List(Table output, string myname)
        {
            yield return Run(output, "FriendList(myname)", "myname", myname);
        }

        /// <summary>
        /// List my friends who are online
        /// Accessors ["friend"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="myname"></param>
        /// <returns></returns>
        IEnumerator IFriendMethods.ListOnline(Table output, string myname)
        {
            yield return Run(output, "FriendListOnline(name)", "name", myname);
        }

        /// <summary>
        /// List my friends who are offline
        /// Accessors ["friend"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="myname"></param>
        /// <returns></returns>
        IEnumerator IFriendMethods.ListOffline(Table output, string myname)
        {
            yield return Run(output, "FriendListOffline(name)", "name", myname);
        }

        /// <summary>
        /// Delete a friend.
        /// </summary>
        /// <param name="myname"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        IEnumerator IFriendMethods.Delete(string myname, string other)
        {
            yield return Run("FriendDelete(myname,other)", "myname", myname, "other", other);
        }
    }
}