namespace UI
{
    public class ConfigureGameMenu : Menu
    {
	    protected override void Update()
        {
            base.Update();
	    }

        public override void Back()
        {
            base.Back();
            menuSystem.uiSystem.systemMediator.dataSystem.networkSystem.StopModule();
        }
    }
}