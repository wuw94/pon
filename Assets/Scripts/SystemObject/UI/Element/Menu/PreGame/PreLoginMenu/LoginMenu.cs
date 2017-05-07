namespace UI
{
    using System.Collections;
    using UnityEngine;

    public class LoginMenu : PreLoginMenu
    {
        private const string FOCUS_TEXTFIELD_USERNAME = "QueryUsername";
        private const string FOCUS_TEXTFIELD_PASSWORD = "QueryPassword";

        private VerifyText username;
        private VerifyText password;

        public LoginMenu(MenuSystem menu_system) : base(menu_system)
        {
            focus = FOCUS_TEXTFIELD_USERNAME;
        }

        public override void OnMenuEnter()
        {
            base.OnMenuEnter();

            username = new VerifyText();
            password = new VerifyText();
        }

        public override void OnGUI()
        {
            base.OnGUI();
        
            QueryGroup();
            ShowErrors();
            BackButton("<-", "Quit to Desktop");
            //Login("a");
        }

        private void QueryGroup()
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.LowerCenter, new Size(0.2f, 0.5f), new Offset(0, 0.1f));
            GUI.BeginGroup(r);

            TextAndTextField(r, 0, "USERNAME", FOCUS_TEXTFIELD_USERNAME, username);
            TextAndPasswordField(r, 0.15f, "PASSWORD", FOCUS_TEXTFIELD_PASSWORD, password);
            Button(r, CanLogin(), new Size(1, 0.1f), 0.35f, "Login", LoginButton);
            Button(r, true, new Size(0.5f, 0.05f), 0.5f, "Create Account", CreateAccountButton);

            GUI.EndGroup();
        }

        private bool CanLogin()
        {
            return username.text != "" && password.text != "" && (username.has_changed || password.has_changed);
        }

        private void ShowErrors()
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.LowerCenter, new Size(0.2f, 0.5f), new Offset(0.2f, 0.1f));
            GUI.BeginGroup(r);
            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0.05f, 0.05f)), username.error, StyleErrorLabel);
            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0.05f, 0.2f)), password.error, StyleErrorLabel);
            GUI.EndGroup();
        }
        
        

        private void LoginButton()
        {
            uiSystem.systemObject.CoroutineStart(CheckLogin());
        }

        private void CreateAccountButton()
        {
            menuSystem.current = typeof(AccountCreationMenu);
        }


        // Data operation
        private IEnumerator CheckLogin()
        {
            username.valid = false;
            password.valid = false;
        
            username.has_changed = false;
            password.has_changed = false;

            if (Database.Validation.IsValidName(username.text))
            {
                // Check if username exists
                username.error = " Verifying...";

                Database.MySQL.Table output = new Database.MySQL.Table();
                yield return uiSystem.systemObject.dataSystem.query.Account.List(output, username.text);

                if (output.success && output.Length == 1)
                {
                    username.valid = true;
                    username.error = "";

                    // Check login values against server
                    string encrypted_password = Database.Validation.Md5Sum(password.text);

                    yield return uiSystem.systemObject.dataSystem.query.Account.List(output, username.text, encrypted_password);

                    if (output.success && output.Length == 1)
                    {
                        Login(output["name"][0]);
                    }
                    else
                    {
                        password.valid = false;
                        password.error = "Incorrect password";
                    }
                }
                else
                {
                    username.valid = false;
                    username.error = "No such username";
                }
            }
            else
            {
                username.error = "Invalid";
            }
        }

        private void Login(string name)
        {
            uiSystem.systemObject.dataSystem.Login(name);
            menuSystem.current = typeof(StandbyMenu);
            Debug.Log("Login for user [" + name + "].");
        }

        protected override void OnKeyEnter()
        {
            base.OnKeyEnter();

            if (GUI.GetNameOfFocusedControl() == FOCUS_TEXTFIELD_PASSWORD)
                if (username.text != "" && password.text != "" && (username.has_changed || password.has_changed))
                    LoginButton();
        }

        protected override void Back()
        {
            base.Back();
            Application.Quit();
        }
    }
}