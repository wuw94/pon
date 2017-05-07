/*
using UnityEngine;
using System.Collections;
using System;

public class MenuInGameGameplay : Menu
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
        Texture2D cursor_image = Resources.Load<Texture2D>("GUI Skins/Cursor/Cursor");
        Cursor.SetCursor(cursor_image, new Vector2(cursor_image.width / 2, cursor_image.height / 2), CursorMode.ForceSoftware);
    }

    public override void OnKeyEscape()
    {
        MenuManager.current = typeof(MenuInGameMenuHome);
    }

    public override void GUIDebug()
    {
    }
}
*/