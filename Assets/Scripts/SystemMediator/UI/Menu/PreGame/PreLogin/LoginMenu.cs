namespace UI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class LoginMenu : Menu
    {
        public InputField UsernameInput;
        public Text UsernameErrorText;
        public InputField PasswordInput;
        public Text PasswordErrorText;
        public Button LoginButton;
        public Text LoginButtonText;
        public Button CreateAccountButton;

        public Menu LoginNextMenu;


	    protected override void Update()
        {
            base.Update();

            if (LoginButton.interactable)
                LoginButtonText.color = Color.black;
            else
                LoginButtonText.color = Color.white;

            if (LoginButton.interactable && EventSystem.current.currentSelectedGameObject == PasswordInput.gameObject && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
                TryLogin();
	    }

        public void OnValueChanged(string newValue)
        {
            LoginButton.interactable = newValue != "";
            UsernameErrorText.text = "";
            PasswordErrorText.text = "";
        }

        private void Login(string name)
        {
            menuSystem.uiSystem.systemMediator.dataSystem.databaseSystem.Login(name);
            FindObjectOfType<MenuSystem>().Push(LoginNextMenu);
            Debug.Log("Login for user [" + name + "].");
        }

        public void TryLogin()
        {
            LoginButton.interactable = false;
            StartCoroutine(TryLoginRoutine());
        }

        private IEnumerator TryLoginRoutine()
        {
            if (Data.Validation.IsValidName(UsernameInput.text))
            {
                // Check if username exists
                UsernameErrorText.text = " Verifying...";

                Data.Database.MySQL.Table output = new Data.Database.MySQL.Table();
                yield return menuSystem.uiSystem.systemMediator.dataSystem.databaseSystem.query.Account.List(output, UsernameInput.text);

                if (output.success && output.Length == 1)
                {
                    UsernameErrorText.text = "";

                    // Check login values against server
                    string encrypted_password = Data.Validation.Md5Sum(PasswordInput.text);

                    yield return menuSystem.uiSystem.systemMediator.dataSystem.databaseSystem.query.Account.List(output, UsernameInput.text, encrypted_password);

                    if (output.success && output.Length == 1)
                    {
                        Login(output["name"][0]);
                    }
                    else
                    {
                        LoginButton.interactable = false;
                        PasswordErrorText.text = "Incorrect password";
                        EventSystem.current.SetSelectedGameObject(PasswordInput.gameObject, new BaseEventData(EventSystem.current));
                    }
                }
                else
                {
                    LoginButton.interactable = false;
                    UsernameErrorText.text = "No such username";
                    EventSystem.current.SetSelectedGameObject(UsernameInput.gameObject, new BaseEventData(EventSystem.current));
                }
            }
            else
            {
                UsernameErrorText.text = "Invalid";
            }
        }
    }
}