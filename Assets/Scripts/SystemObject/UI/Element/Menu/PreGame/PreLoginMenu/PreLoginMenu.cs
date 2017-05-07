namespace UI
{
    using UnityEngine;

    public abstract class PreLoginMenu : PreGameMenu
    {
        protected GUIStyle StyleOrangeButton = Resources.Load<Style>("Menu/PreGame/PreLogin/OrangeButton/style");
        protected GUIStyle StyleOrangeButtonInactive = Resources.Load<Style>("Menu/PreGame/PreLogin/OrangeButtonInactive/style");
        protected GUIStyle StyleQueryLabel = Resources.Load<Style>("Menu/PreGame/PreLogin/Query/Label/style");
        protected GUIStyle StyleQueryTextField = Resources.Load<Style>("Menu/PreGame/PreLogin/Query/TextField/style");
        protected GUIStyle StyleErrorLabel = Resources.Load<Style>("Menu/PreGame/PreLogin/Error/style");

        protected delegate void VoidDelegate();

        public PreLoginMenu(MenuSystem menu_system) : base(menu_system)
        {
        }

        public override void Update()
        {
            base.Update();

            StyleOrangeButton.fontSize = uiSystem.fontSize;
            StyleOrangeButtonInactive.fontSize = uiSystem.fontSize;
            StyleQueryLabel.fontSize = uiSystem.fontSize / 2;
            StyleQueryTextField.fontSize = uiSystem.fontSize * 4 / 5;
            StyleErrorLabel.fontSize = uiSystem.fontSize / 2;
        }

        protected void TextAndTextField(Rect r, float offsetY, string text, string controlName, VerifyText verifyText)
        {
            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.05f), new Offset(0, offsetY)), text, StyleQueryLabel);

            GUI.SetNextControlName(controlName);
            string textNew = GUI.TextField(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0, offsetY + 0.05f)), verifyText.text, 15, StyleQueryTextField);
            if (verifyText.text != textNew)
            {
                verifyText.has_changed = true;
                verifyText.valid = false;
            }
            verifyText.text = textNew;
            if (verifyText.has_changed)
                verifyText.error = "";
        }

        protected void TextAndPasswordField(Rect r, float offsetY, string text, string controlName, VerifyText verifyText)
        {
            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.05f), new Offset(0, offsetY)), text, StyleQueryLabel);

            GUI.SetNextControlName(controlName);
            string textNew = GUI.PasswordField(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0, offsetY + 0.05f)), verifyText.text, "•"[0], 15, StyleQueryTextField);
            if (verifyText.text != textNew)
            {
                verifyText.has_changed = true;
                verifyText.valid = false;
            }
            verifyText.text = textNew;
            if (verifyText.has_changed)
                verifyText.error = "";
        }

        protected void Button(Rect r, bool active, Size size, float offsetY, string text, VoidDelegate voidDelegate)
        {
            StyleOrangeButton.fontSize = (int)(uiSystem.fontSize * size.x);
            StyleOrangeButtonInactive.fontSize = (int)(uiSystem.fontSize * size.x);
            if (active)
            {
                if (GUI.Button(Container.GetScaled(r, Anchor.UpperCenter, size, new Offset(0, offsetY)), text, StyleOrangeButton))
                {
                    voidDelegate();
                }
            }
            else
            {
                GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, size, new Offset(0, offsetY)), text, StyleOrangeButtonInactive);
            }
        }

        protected class VerifyText
        {
            public string text = "";
            public string error = "";

            public bool valid = false;
            public bool has_changed = false;

            public VerifyText()
            {
            }
        }
    }
}