namespace UI
{
    using UnityEngine;

    public partial class InvitationManager
    {
        private class Invitation
        {
            private SystemObject systemObject;
            public string sender;

            public Invitation(SystemObject systemObject, string sender)
            {
                this.systemObject = systemObject;
                this.sender = sender;
            }

            public void OnGUI()
            {
                Rect r = Container.GetScaled(Container.screen, Anchor.MiddleCenter, new Size(0.4f, 0.4f));
                GUI.BeginGroup(r);
                GUI.Box(new Rect(Vector2.zero, r.size), "");
                GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(1, 0.5f)), "You've been invited to a party by " + sender + ". Do you want to join?");

                if (GUI.Button(Container.GetScaled(r, Anchor.LowerLeft, new Size(0.4f, 0.4f)), "Yes"))
                {
                    RequestAccept();
                }
                else if (GUI.Button(Container.GetScaled(r, Anchor.LowerRight, new Size(0.4f, 0.4f)), "No"))
                {
                    RequestReject();
                }
                GUI.EndGroup();
            }

            public void RequestAccept()
            {
                systemObject.uiSystem.notificationSystem.Remove(new NotificationObject(this, OnGUI));
                systemObject.CoroutineStart(systemObject.dataSystem.query.Party.Join(systemObject.dataSystem.profile.userName, sender));
                systemObject.CoroutineStart(systemObject.dataSystem.partyInvitation.RemoveInvitation(sender));
            }

            public void RequestReject()
            {
                systemObject.uiSystem.notificationSystem.Remove(new NotificationObject(this, OnGUI));
                systemObject.CoroutineStart(systemObject.dataSystem.partyInvitation.RemoveInvitation(sender));
            }

            public override bool Equals(object obj)
            {
                return sender == (obj as Invitation).sender;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}