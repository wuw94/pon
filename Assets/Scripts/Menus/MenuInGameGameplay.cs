using UnityEngine;
using System.Collections;
using System;

public class MenuInGameGameplay : Menu
{
    public override void RunGUI()
    {
        Texture2D cursor_image = Resources.Load<Texture2D>("GUI Skins/Cursor/Cursor");
        Cursor.SetCursor(cursor_image, new Vector2(cursor_image.width / 2, cursor_image.height / 2), CursorMode.ForceSoftware);
    }

    public override void Esc()
    {
        MenuManager.current_menu = typeof(MenuInGameMenuHome);
    }
}
