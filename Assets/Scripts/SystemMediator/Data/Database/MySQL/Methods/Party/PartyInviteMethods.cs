namespace Data.Database.MySQL
{
    using System.Collections;

    public interface IPartyInviteMethods
    {
        IEnumerator ListSender(Table output, string sender);
        IEnumerator ListReceiver(Table output, string receiver);
        IEnumerator Make(string sender, string receiver);
        IEnumerator Remove(string sender, string receiver);
    }

    public partial class Query : IPartyInviteMethods
    {
        public IPartyInviteMethods PartyInvite { get { return this as IPartyInviteMethods; } }

        /// <summary>
        /// List people who I've sent party invites to.
        /// Accessors ["name"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        IEnumerator IPartyInviteMethods.ListSender(Table output, string sender)
        {
            yield return Run(output, "PartyInviteListSender(sender)", "sender", sender);
        }

        /// <summary>
        /// List all party invitations to me.
        /// Accessors ["sender"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        IEnumerator IPartyInviteMethods.ListReceiver(Table output, string receiver)
        {
            yield return Run(output, "PartyInviteListReceiver(receiver)", "receiver", receiver);
        }

        /// <summary>
        /// Make a party invitation.
        /// </summary>
        /// <param name="old_leader"></param>
        /// <param name="new_leader"></param>
        /// <returns></returns>
        IEnumerator IPartyInviteMethods.Make(string sender, string receiver)
        {
            yield return Run("PartyInviteMake(sender,receiver)", "sender", sender, "receiver", receiver);
        }

        /// <summary>
        /// Remove a party invitation.
        /// </summary>
        /// <param name="old_leader"></param>
        /// <param name="new_leader"></param>
        /// <returns></returns>
        IEnumerator IPartyInviteMethods.Remove(string sender, string receiver)
        {
            yield return Run("PartyInviteRemove(sender,receiver)", "sender", sender, "receiver", receiver);
        }
    }
}