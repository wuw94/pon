using UnityEngine;
using System.Collections;
using System;

public class MenuPreGameHome : Menu
{
    public override void RunGUI()
    {
        GUISkin MenuSkin = Resources.Load<GUISkin>("GUI Skins/MenuGuiSkin");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GUI.skin = MenuSkin;
        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");
        GUI.Label(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "pon", MenuSkin.FindStyle("Title"));


        // Your Name
        GUI.Label(new Rect(PG_GROUP_WIDTH / 2 - 75, 100, 150, 30), "Your Name");
        Settings.PLAYER_NAME = GUI.TextField(new Rect(PG_GROUP_WIDTH / 2 - 75, 120, 150, 25), Settings.PLAYER_NAME, 15);
        if (!Settings.IsValidName(Settings.PLAYER_NAME))
            GUI.Label(new Rect(PG_GROUP_WIDTH / 2 + 80, 120, 150, 30), "name not allowed!");

        // Create Game
        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 60, 160, 120, 30), "Host") && Settings.IsValidName(Settings.PLAYER_NAME))
        {
            MenuManager.current_menu = typeof(MenuInGameConfigureGame);
            MonoBehaviour.FindObjectOfType<Server>().CreateInternetMatch(Settings.PLAYER_NAME);
        }

        // Join Game
        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 60, 190, 120, 30), "Join") && Settings.IsValidName(Settings.PLAYER_NAME))
            MenuManager.current_menu = typeof(MenuPreGameListing);

        // Quit Game
        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 60, 330, 120, 30), "Quit"))
            Application.Quit();

        GUI.EndGroup();
    }

    public override void Esc()
    {
    }
}
