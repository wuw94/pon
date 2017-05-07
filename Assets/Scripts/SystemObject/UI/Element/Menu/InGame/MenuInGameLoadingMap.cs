/*
using UnityEngine;
using System.Collections;
using System;

public class MenuInGameLoadingMap : Menu
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
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GUI.skin = MenuSkin;
        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");

        GUI.Label(new Rect(0, PG_GROUP_HEIGHT / 2 - 10, PG_GROUP_WIDTH, 30), Globals.GetLoadingWords());

        GUI.EndGroup();
    }

    public override void OnKeyEscape()
    {
        Application.Quit();
    }

    public override void GUIDebug()
    {
    }
}
*/