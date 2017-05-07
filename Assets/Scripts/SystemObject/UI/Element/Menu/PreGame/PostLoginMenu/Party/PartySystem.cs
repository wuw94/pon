namespace UI
{
    public class PartySystem : SystemBase
    {
        private PartyUI party;
        private InvitationManager partyInvitation;

        public PartySystem(MenuSystem menuSystem)
        {
            party = new PartyUI(menuSystem);
            partyInvitation = new InvitationManager(menuSystem);
        }

        public override void Update()
        {
            party.Update();
            partyInvitation.Update();
        }

        public override void OnGUI()
        {
            party.OnGUI();
        }
    }
}