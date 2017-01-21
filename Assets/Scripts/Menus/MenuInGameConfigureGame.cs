using UnityEngine;
using System.Collections.Generic;
using System;

public class MenuInGameConfigureGame : Menu
{
    public override void RunGUI()
    {
        GUISkin MenuSkin = Resources.Load<GUISkin>("GUI Skins/MenuGuiSkin");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GUI.skin = MenuSkin;
        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");


        if (GUI.Button(new Rect(20, PG_GROUP_HEIGHT - 50, 60, 30), "Back"))
            Esc();


        GUI.Label(new Rect(0, 40, PG_GROUP_WIDTH / 3, 30), "Attacking");
        GUI.Label(new Rect(PG_GROUP_WIDTH * 2 / 3, 40, PG_GROUP_WIDTH / 3, 30), "Defending");

        if (Player.mine == null)
        {
            GUI.EndGroup();
            return;
        }

        Player[] players = MonoBehaviour.FindObjectsOfType<Player>();
        List<Player> attacking_players = new List<Player>();
        List<Player> defending_players = new List<Player>();
        List<Player> neutral_players = new List<Player>();
        foreach (Player p in players)
        {
            if (p.selected_team == Team.Neutral)
                neutral_players.Add(p);
            else if (p.selected_team == Team.A)
                attacking_players.Add(p);
            else
                defending_players.Add(p);
        }


        // Attacking Players
        for (int i = 0; i < attacking_players.Count; i++)
        {
            GUI.Label(new Rect(0, 80 + 30 * i, PG_GROUP_WIDTH / 3, 30), attacking_players[i].name);
        }
        if (Player.mine.selected_team != Team.A)
            if (GUI.Button(new Rect(20, 80 + 30 * attacking_players.Count, PG_GROUP_WIDTH / 3 - 40, 30), "Join Team"))
                Player.mine.CmdChangeSelectedTeam(Team.A);

        // Defending Players
        for (int i = 0; i < defending_players.Count; i++)
        {
            GUI.Label(new Rect(PG_GROUP_WIDTH * 2 / 3, 80 + 30 * i, PG_GROUP_WIDTH / 3, 30), defending_players[i].name);
        }
        if (Player.mine.selected_team != Team.B)
            if (GUI.Button(new Rect(PG_GROUP_WIDTH * 2 / 3 + 20, 80 + 30 * defending_players.Count, PG_GROUP_WIDTH / 3 - 40, 30), "Join Team"))
                Player.mine.CmdChangeSelectedTeam(Team.B);

        for (int i = 0; i < neutral_players.Count; i++)
            GUI.Label(new Rect(PG_GROUP_WIDTH * 1 / 3, 80 + 30 * i, PG_GROUP_WIDTH / 3, 30), neutral_players[i].name);



        if (Player.mine.is_host && neutral_players.Count == 0)
            if (GUI.Button(new Rect(PG_GROUP_WIDTH - 80, PG_GROUP_HEIGHT - 50, 60, 30), "Start"))
            {
                MonoBehaviour.FindObjectOfType<Server>().UnlistMatch();
                MapGenerator map_generator = MonoBehaviour.FindObjectOfType<MapGenerator>();
                map_generator.StartCoroutine(map_generator.BeginGeneration());
            }
        
        GUI.EndGroup();
    }

    public override void Esc()
    {
        if (Settings.WAIT_FOR.Count > 0)
            return;
        if (Player.mine != null)
        {
            if (Player.mine.is_host)
            {
                Settings.WAIT_FOR.Add(Settings.WaitTypes.DESTROY_MATCH_CALLBACK);
                MonoBehaviour.FindObjectOfType<Server>().DestroyInternetMatch();
                if (MonoBehaviour.FindObjectOfType<Server>().LAN_mode)
                    MonoBehaviour.FindObjectOfType<Server>().ResetToHome();
            }
            else
            {
                MonoBehaviour.FindObjectOfType<Server>().ResetToHome();
            }
        }
    }
}
