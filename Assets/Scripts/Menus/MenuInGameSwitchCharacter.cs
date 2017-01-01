using UnityEngine;
using System.Collections;
using System;

public class MenuInGameSwitchCharacter : Menu
{
    public override void RunGUI()
    {
        GUISkin MenuSkin = Resources.Load<GUISkin>("GUI Skins/MenuGuiSkin");
        Texture2D MenuOverlay = Resources.Load<Texture2D>("GUI Skins/MenuOverlay");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MenuOverlay);
        GUI.skin = MenuSkin;

        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");
        GUI.Label(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "Switch Character", MenuSkin.FindStyle("MenuTitle"));


        GUI.Label(new Rect(0, PG_GROUP_HEIGHT / 2, PG_GROUP_WIDTH, 30), Player.mine.possible_characters[Player.mine.selected_character].name);

        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 70, PG_GROUP_HEIGHT / 2, 30, 30), "<"))
            Player.mine.CmdPreviousCharacter();
        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 + 50, PG_GROUP_HEIGHT / 2, 30, 30), ">"))
            Player.mine.CmdNextCharacter();


        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 60, 355, 120, 30), "Done (Esc)"))
            Esc();

        GUI.EndGroup();
    }

    public override void Esc()
    {
        MenuManager.current_menu = typeof(MenuInGameGameplay);
        Player.mine.SwitchCharacter(Player.mine.selected_character);
    }
}
