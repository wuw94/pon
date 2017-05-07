/*
using UnityEngine;
using System.Collections;
using System;

public class MenuInGameMenuHome : Menu
{
    public override void OnMenuEnter()
    {

    }

    public override void OnMenuExit()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void OnGUI()
    {
        GUISkin MenuSkin = Resources.Load<GUISkin>("GUI Skins/MenuGuiSkin");
        Texture2D MenuOverlay = Resources.Load<Texture2D>("GUI Skins/MenuOverlay");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MenuOverlay);
        GUI.skin = MenuSkin;

        GUI.BeginGroup(new Rect(20, Screen.height / 2 - IG_GROUP_HEIGHT / 2, IG_GROUP_WIDTH, IG_GROUP_HEIGHT)); // Screen.width / 2 - IG_GROUP_WIDTH / 2
        GUI.Box(new Rect(0, 0, IG_GROUP_WIDTH, IG_GROUP_HEIGHT), "");
        GUI.Label(new Rect(0, 0, IG_GROUP_WIDTH, IG_GROUP_HEIGHT), "menu", MenuSkin.FindStyle("MenuTitle"));


        if (GUI.Button(new Rect(15, 50, IG_GROUP_WIDTH - 30, 30), "Back to Game"))
        {
            OnKeyEscape();
        }

        if (GUI.Button(new Rect(15, 80, IG_GROUP_WIDTH - 30, 30), "Switch Character") && Player.mine.can_choose_character)
        {
            MenuManager.current = typeof(MenuInGameSwitchCharacter);
        }

        if (GUI.Button(new Rect(15, 110, IG_GROUP_WIDTH - 30, 30), "N/A"))
        {
        }


        if (Player.mine.is_host)
            if (GUI.Button(new Rect(15, 325, IG_GROUP_WIDTH - 30, 30), "Swap Sides"))
            {
                MonoBehaviour.FindObjectOfType<MapGenerator>().Reset();
                OnKeyEscape();
            }

        if (GUI.Button(new Rect(15, 355, IG_GROUP_WIDTH - 30, 30), "Quit"))
        {
            MonoBehaviour.FindObjectOfType<Server>().ResetToHome();
        }
        GUI.EndGroup();
    }

    public override void OnKeyEscape()
    {
        MenuManager.current = typeof(MenuInGameGameplay);
    }

    public override void GUIDebug()
    {
    }
}
*/