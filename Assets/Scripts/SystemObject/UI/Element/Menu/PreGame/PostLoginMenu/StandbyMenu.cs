namespace UI
{
    using UnityEngine;

    public class StandbyMenu : PostLoginMenu
    {
        private Vector2 menu_group_dim;
        private const int menu_group_num_elem = 24;

        public StandbyMenu(MenuSystem menu_system) : base(menu_system)
        {
        }

        public override void OnGUI()
        {
            base.OnGUI();
        
            MenuGroup();

            BackButton("<-", "Quit to Desktop");
        }

        private void MenuGroup()
        {

            menu_group_dim = new Vector2(Mathf.Min(600, 600), Mathf.Min(300, 300));
            GUI.BeginGroup(new Rect(new Vector2((Screen.width - menu_group_dim.x) / 2, (Screen.height - menu_group_dim.y) / 2), menu_group_dim));

            StyleOrangeButton.fontSize = uiSystem.fontSize * 2;
            PlayButton();
            StyleOrangeButton.fontSize = uiSystem.fontSize / 2;
            FriendsButton();
            ProfileButton();
            TutorialButton();
            CustomButton();
            OptionsButton();

            GUI.EndGroup();
        }

        private void PlayButton()
        {
            if (GUI.Button(GetDividedRect(menu_group_dim, menu_group_num_elem, menu_group_num_elem, 7, 8, 10, 8), "Play", StyleOrangeButton))
            {

            }
        }
    
        private void FriendsButton()
        {
            if (GUI.Button(GetDividedRect(menu_group_dim, menu_group_num_elem, menu_group_num_elem, 0, 19, 3, 3), "Friends", StyleOrangeButton))
            {
                menuSystem.current = typeof(FriendsMenu);
            }
        }

        private void ProfileButton()
        {
            if (GUI.Button(GetDividedRect(menu_group_dim, menu_group_num_elem, menu_group_num_elem, 4, 19, 3, 3), "Profile", StyleOrangeButton))
            {

            }
        }

        private void TutorialButton()
        {
            if (GUI.Button(GetDividedRect(menu_group_dim, menu_group_num_elem, menu_group_num_elem, 9, 18, 6, 5), "Tutorial", StyleOrangeButton))
            {

            }
        }

        private void CustomButton()
        {
            if (GUI.Button(GetDividedRect(menu_group_dim, menu_group_num_elem, menu_group_num_elem, 17, 19, 3, 3), "Custom", StyleOrangeButton))
            {

            }
        }
    
        private void OptionsButton()
        {
            if (GUI.Button(GetDividedRect(menu_group_dim, menu_group_num_elem, menu_group_num_elem, 21, 19, 3, 3), "Options", StyleOrangeButton))
            {

            }
        }


        private Rect GetDividedRect(Vector2 group, int totalX, int totalY, float posX, float posY, float sizeX, float sizeY)
        {
            return new Rect(group.x * posX / totalX, group.y * posY / totalY, group.x * sizeX / totalX, group.y * sizeY / totalY);
        }

        protected override void Back()
        {
            base.Back();
            Application.Quit();
        }
    }
}