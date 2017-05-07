namespace UI
{
    using UnityEngine;

    public abstract class PreGameMenu : Menu
    {
        private readonly Color SELECTION_COLOR = new Color(1, 201 / 255.0f, 51 / 255.0f);
        private GUIStyle StyleBackLabel = Resources.Load<Style>("Menu/PreGame/Back/Label/style");
        private GUIStyle StyleBackButton = Resources.Load<Style>("Menu/PreGame/Back/Button/style");

        public PreGameMenu(MenuSystem menuSystem) : base(menuSystem)
        {
        }

        public override void Update()
        {
            base.Update();

            StyleBackButton.fontSize = StyleBackLabel.fontSize = uiSystem.fontSize / 2;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            GUI.skin.settings.cursorColor = Color.black;
            GUI.skin.settings.selectionColor = SELECTION_COLOR;
        }

        /// <summary>
        /// Display Back Button
        /// </summary>
        /// <param name="button_text"></param>
        /// <param name="comment_text"></param>
        protected void BackButton(string button_text, string comment_text)
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.LowerLeft, new Size(0.5f, 0.04f), new Offset(0.02f, -0.02f));
            GUI.BeginGroup(r);

            if (GUI.Button(Container.GetScaled(r, Anchor.MiddleLeft, new Size(0.1f, 1)), button_text, StyleBackButton))
                Back();

            GUI.Label(Container.GetScaled(r, Anchor.MiddleLeft, new Size(0.89f, 1), new Offset(0.11f, 0)), comment_text, StyleBackLabel);
            GUI.EndGroup();
        }
    }
}