namespace UI
{
    using UnityEngine;

    public abstract class PostLoginMenu : PreGameMenu
    {
        private GUIStyle StyleTitleLabel = Resources.Load<Style>("Menu/PreGame/PostLogin/Title/style");
        protected string title = "";

        protected GUIStyle StyleOrangeButton = Resources.Load<Style>("Menu/PreGame/PostLogin/OrangeButton/style");
        protected GUIStyle StyleSearchTextField = Resources.Load<Style>("Menu/PreGame/PostLogin/Search/style");

        private PartySystem partySystem;

        public PostLoginMenu(MenuSystem menuSystem) : base(menuSystem)
        {
            partySystem = new PartySystem(menuSystem);
        }

        public override void Update()
        {
            base.Update();

            partySystem.Update();
            StyleTitleLabel.fontSize = uiSystem.fontSize * 3 / 2;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            partySystem.OnGUI();
            Title(title);
            GUIChatBox();
        }

        /// <summary>
        /// Display Title
        /// </summary>
        /// <param name="text"></param>
        protected void Title(string text)
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.UpperLeft, new Size(0.48f, 0.08f), new Offset(0.02f, 0.02f));
            GUI.BeginGroup(r);
            UISystem.ShowContainer(r);
            GUI.Label(Container.GetScaled(r), text, StyleTitleLabel);
            GUI.EndGroup();
        }

        /// <summary>
        /// Display Chat Box
        /// </summary>
        protected void GUIChatBox()
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.LowerLeft, new Size(0.23f, 0.2f), new Offset(0.02f, -0.08f));
            GUI.BeginGroup(r);
            UISystem.ShowContainer(r);
            GUI.Label(Container.GetScaled(r), "Chatbox");
            GUI.EndGroup();
        }
    }
}