namespace UI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class CreateAccountMenu : Menu
    {
        public InputField UsernameInput;
        public Text UsernameErrorText;
        public InputField PasswordInput;
        public Text PasswordErrorText;
        public InputField ConfirmPasswordInput;
        public Text ConfirmPasswordErrorText;
        public InputField EmailInput;
        public Text EmailErrorText;

        public Button CheckValidButton;
        public Button CreateAccountButton;
        public Text CreateAccountButtonText;

        private bool valid = false;

        protected override void Update()
        {
            base.Update();

            CheckValidButton.gameObject.SetActive(!valid);
            CreateAccountButton.interactable = valid;
            CreateAccountButtonText.color = CreateAccountButton.interactable ? Color.black : Color.white;
        }

        public void OnValueChanged(string newValue)
        {
            valid = false;
            if (UsernameInput.text == "" || PasswordInput.text == "" || ConfirmPasswordInput.text == "" || EmailInput.text == "")
                CreateAccountButton.interactable = false;
            else
                CreateAccountButton.interactable = newValue != "";
            UsernameErrorText.text = "";
            PasswordErrorText.text = "";
            ConfirmPasswordErrorText.text = "";
            EmailErrorText.text = "";
        }


        public void CheckValid()
        {
            StartCoroutine(CheckValidRoutine());
        }

        // Data operation
        private IEnumerator CheckValidRoutine()
        {
            valid = false;
            if (Data.Validation.IsValidName(UsernameInput.text))
            {
                UsernameErrorText.text = " Verifying...";

                Data.Database.MySQL.Table output = new Data.Database.MySQL.Table();
                yield return menuSystem.uiSystem.systemMediator.dataSystem.databaseSystem.query.Account.List(output, UsernameInput.text);

                if (output.success) // we need to check if sql was successful
                {
                    UsernameErrorText.text = (output.Length == 0) ? "OK" : "Unavailable";
                }
            }
            else
            {
                UsernameErrorText.text = "Invalid";
            }

            PasswordErrorText.text = (PasswordInput.text == ConfirmPasswordInput.text) ? "OK" : "Passwords must match";
            ConfirmPasswordErrorText.text = PasswordErrorText.text;
            EmailErrorText.text = (Data.Validation.IsValidEmail(EmailInput.text)) ? "OK" : "Invalid";
            if (UsernameErrorText.text == "OK" && PasswordErrorText.text == "OK" && ConfirmPasswordErrorText.text == "OK" && EmailErrorText.text == "OK")
                valid = true;
        }

        public void CreateAccount()
        {
            Debug.Log("Account not actually created for testing reasons");
            FindObjectOfType<MenuSystem>().Back();
        }

        // Back-end operation
        private IEnumerator CreateAccount(string name, string password, string email)
        {
            yield return menuSystem.uiSystem.systemMediator.dataSystem.databaseSystem.query.Account.Create(name, password, email);
            FindObjectOfType<MenuSystem>().Back();
        }
    }
}