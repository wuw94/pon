namespace Database.MySQL
{
    using System.Collections;

    public interface IFriendRequestMethods
    {
        IEnumerator Make(string requester, string receiver);
        IEnumerator Cancel(string requester, string receiver);
        IEnumerator Reject(string receiver, string requester);
        IEnumerator Accept(string receiver, string requester);
        IEnumerator ListRequester(Table output, string requester);
        IEnumerator ListReceiver(Table output, string receiver);
    }

    public partial class Query : IFriendRequestMethods
    {
        public IFriendRequestMethods FriendRequest { get { return this as IFriendRequestMethods; } }

        /// <summary>
        /// Make a friend request.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        IEnumerator IFriendRequestMethods.Make(string requester, string receiver)
        {
            yield return Run("FriendRequestMake(requester,receiver)", "requester", requester, "receiver", receiver);
        }

        /// <summary>
        /// Cancel a friend request.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        IEnumerator IFriendRequestMethods.Cancel(string requester, string receiver)
        {
            yield return Run("FriendRequestCancel(requester,receiver)", "requester", requester, "receiver", receiver);
        }

        /// <summary>
        /// Reject a friend request.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        IEnumerator IFriendRequestMethods.Reject(string receiver, string requester)
        {
            yield return Run("FriendRequestReject(receiver,requester)", "receiver", receiver, "requester", requester);
        }

        /// <summary>
        /// Accept a friend request.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        IEnumerator IFriendRequestMethods.Accept(string receiver, string requester)
        {
            yield return Run("FriendRequestAccept(receiver,requester)", "receiver", receiver, "requester", requester);
        }

        /// <summary>
        /// List the names of people who I'm requesting to be friends with.
        /// Accessors ["name"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        IEnumerator IFriendRequestMethods.ListRequester(Table output, string requester)
        {
            yield return Run(output, "FriendRequestListRequester(requester)", "requester", requester);
        }

        /// <summary>
        /// List the names of people who are requesting to be friends with me.
        /// Accessors ["name"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        IEnumerator IFriendRequestMethods.ListReceiver(Table output, string receiver)
        {
            yield return Run(output, "FriendRequestListReceiver(receiver)", "receiver", receiver);
        }
    }
}