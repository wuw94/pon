using UnityEngine;
using UnityEngine.Networking;

public class MenuPreGameListing : Menu
{
    private int current_selection = 0;
    private string[] selection_strings = { };

    public override void RunGUI()
    {
        GUISkin MenuSkin = Resources.Load<GUISkin>("GUI Skins/MenuGuiSkin");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GUI.skin = MenuSkin;
        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");


        if (GUI.Button(new Rect(20, PG_GROUP_HEIGHT - 50, 60, 30), "Back"))
            Esc();
        GUI.Label(new Rect(PG_GROUP_WIDTH / 2 - 150, 40, 300, 30), "Hello [" + Settings.PLAYER_NAME + "]. Here are your matches");

        MonoBehaviour.FindObjectOfType<Server>().ListInternetMatches();


        if (NetworkManager.singleton.matches != null)
        {
            selection_strings = new string[NetworkManager.singleton.matches.Count];
            for (int i = 0; i < NetworkManager.singleton.matches.Count; i++)
                selection_strings[i] = NetworkManager.singleton.matches[i].name + " " + NetworkManager.singleton.matches[i].currentSize + "/" + NetworkManager.singleton.matches[i].maxSize;
            current_selection = GUI.SelectionGrid(new Rect(PG_GROUP_WIDTH / 2 - 200, 60, 400, selection_strings.Length * 30), current_selection, selection_strings, 1);
        }
        if (selection_strings.Length != 0)
            if (GUI.Button(new Rect(PG_GROUP_WIDTH - 80, PG_GROUP_HEIGHT - 50, 60, 30), "Join"))
            {
                if (NetworkManager.singleton.matches[current_selection].name.StartsWith(Settings.PLAYER_NAME))
                    return;
                Settings.WAIT_FOR.Add(Settings.WaitTypes.JOIN_MATCH);
                MenuManager.current_menu = typeof(MenuInGameConfigureGame);
                MonoBehaviour.FindObjectOfType<Server>().JoinInternetMatch(NetworkManager.singleton.matches[current_selection]);
            }

        GUI.EndGroup();
    }

    public override void Esc()
    {
        current_selection = 0;
        MenuManager.current_menu = typeof(MenuPreGameHome);
    }
}
