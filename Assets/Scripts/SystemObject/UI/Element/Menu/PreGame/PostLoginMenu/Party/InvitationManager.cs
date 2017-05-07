namespace UI
{
    public partial class InvitationManager
    {
        private MenuSystem menuSystem;
        private Database.Mediator.PartyInvitation partyInvitationData;

        public InvitationManager(MenuSystem menuSystem)
        {
            this.menuSystem = menuSystem;
            partyInvitationData = menuSystem.uiSystem.systemObject.dataSystem.partyInvitation;
        }

        public void Update()
        {
            if (partyInvitationData.Has())
            {
                AddInvitation(partyInvitationData.Sender());
            }
            else
            {
                ClearAll();
            }
        }

        private void AddInvitation(string sender)
        {
            Invitation PI = new Invitation(menuSystem.uiSystem.systemObject, sender);
            NotificationObject n = new NotificationObject(PI, PI.OnGUI);
            if (menuSystem.uiSystem.notificationSystem.Contains(typeof(Invitation)) && !menuSystem.uiSystem.notificationSystem.Contains(n))
                ClearAll();
            menuSystem.uiSystem.notificationSystem.Enqueue(n);
        }

        private void ClearAll()
        {
            menuSystem.uiSystem.notificationSystem.Remove(typeof(Invitation));
        }
    }
}