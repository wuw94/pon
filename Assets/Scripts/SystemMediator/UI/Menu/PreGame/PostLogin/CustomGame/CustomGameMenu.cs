namespace UI
{
    public class CustomGameMenu : Menu
    {
	    protected override void Update()
        {
            base.Update();
	    }

        public void Host()
        {
            menuSystem.uiSystem.systemMediator.dataSystem.networkSystem.Host();
        }

        public void Join()
        {
            menuSystem.uiSystem.systemMediator.dataSystem.networkSystem.StartModule();
        }
    }
}