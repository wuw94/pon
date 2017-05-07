namespace UI
{
    using System.Collections;
    using UnityEngine;

    public sealed class AccountCreationMenu : PreLoginMenu
    {
        private const string FOCUS_TEXTFIELD_USERNAME = "QueryUsername";

        private VerifyText username;
        private VerifyText password;
        private VerifyText password_confirm;
        private VerifyText email;

        public AccountCreationMenu(MenuSystem menu_system) : base(menu_system)
        {
            focus = FOCUS_TEXTFIELD_USERNAME;
        }

        public override void OnMenuEnter()
        {
            base.OnMenuEnter();

            username = new VerifyText();
            password = new VerifyText();
            password_confirm = new VerifyText();
            email = new VerifyText();
        }

        public override void OnGUI()
        {
            base.OnGUI();
        
            QueryGroup();
            ShowErrors();

            BackButton("Escape", "Back to Login");
        }

        private void QueryGroup()
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.MiddleCenter, new Size(0.2f, 0.5f), new Offset(0, 0.2f));
            GUI.BeginGroup(r);

            TextAndTextField(r, 0, "USERNAME", FOCUS_TEXTFIELD_USERNAME, username);
            TextAndPasswordField(r, 0.15f, "PASSWORD", "", password);
            TextAndPasswordField(r, 0.3f, "CONFIRM PASSWORD", "", password_confirm);
            TextAndTextField(r, 0.45f, "EMAIL", "", email);

            if (!username.valid || !password.valid || !password_confirm.valid || !email.valid)
            {
                Button(r, CanCheckValid(), new Size(0.5f, 0.1f), 0.65f, "Check Valid", CheckValidButton);
            }
            Button(r, CanCreateAccount(), new Size(1, 0.1f), 0.8f, "Create Account", CreateAccountButton);

            GUI.EndGroup();
        }
        
        private bool CanCheckValid()
        {
            return username.text != "" && password.text != "" && password_confirm.text != "" && email.text != "" && (username.has_changed || password.has_changed || password_confirm.has_changed || email.has_changed);
        }

        private bool CanCreateAccount()
        {
            return username.valid && password.valid && password_confirm.valid && email.valid;
        }

        private void ShowErrors()
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.MiddleCenter, new Size(0.2f, 0.5f), new Offset(0.2f, 0.2f));
            GUI.BeginGroup(r);

            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0.05f, 0.05f)), username.error, StyleErrorLabel);
            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0.05f, 0.2f)), password.error, StyleErrorLabel);
            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0.05f, 0.35f)), password_confirm.error, StyleErrorLabel);
            GUI.Label(Container.GetScaled(r, Anchor.UpperCenter, new Size(1, 0.1f), new Offset(0.05f, 0.5f)), email.error, StyleErrorLabel);

            GUI.EndGroup();
        }

    

        private void CheckValidButton()
        {
            uiSystem.systemObject.CoroutineStart(Verify());
        }

        private void CreateAccountButton()
        {
            uiSystem.systemObject.CoroutineStart(CreateAccount(username.text, Database.Validation.Md5Sum(password.text), email.text));
        }


        // Data operation
        private IEnumerator Verify()
        {
            username.valid = false;
            email.valid = false;

            username.has_changed = false;
            password.has_changed = false;
            password_confirm.has_changed = false;
            email.has_changed = false;

            if (Database.Validation.IsValidName(username.text))
            {
                username.error = " Verifying...";

                Database.MySQL.Table output = new Database.MySQL.Table();
                yield return uiSystem.systemObject.dataSystem.query.Account.List(output, username.text);

                if (output.success) // we need to check if sql was successful
                {
                    username.valid = (output.Length == 0) ? true : false;
                    username.error = (output.Length == 0) ? "OK" : "Unavailable";
                }
            }
            else
            {
                username.error = "Invalid";
            }

            password.valid = password.text == password_confirm.text;
            password.error = (password.text == password_confirm.text) ? "OK" : "Passwords must match";
            password_confirm.valid = password.valid;
            password_confirm.error = password.error;

            email.valid = Database.Validation.IsValidEmail(email.text);
            email.error = (Database.Validation.IsValidEmail(email.text)) ? "OK" : "Invalid";
        }

        // Back-end operation
        private IEnumerator CreateAccount(string name, string password, string email)
        {
            yield return uiSystem.systemObject.dataSystem.query.Account.Create(name, password, email);
            Back();
        }

    

        protected override void OnKeyEscape()
        {
            base.OnKeyEscape();
            Back();
        }

        protected override void Back()
        {
            base.Back();
            menuSystem.current = typeof(LoginMenu);
        }
    }
}