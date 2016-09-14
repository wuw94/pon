using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// These are the possible menu pages in the game. PG = PreGame, IG = InGame. These are based on connection
/// </summary>
public enum MenuPage
{
    PG_Home, PG_Listing, IG_ConfigureGame, IG_LoadingMap, IG_Gameplay, IG_MenuHome,
}

public sealed class Menu : MonoBehaviour
{
    public GUISkin MenuSkin;
    public Texture2D MenuOverlay;
    public static MenuPage current = MenuPage.PG_Home;

    // PG_Home

    // PG_Listing
    private int current_selection = 0;
    private string[] selection_strings = { };


    // IG_ConfigureGame


    public MenuPage c;

    private const int PG_GROUP_WIDTH = 600;
    private const int PG_GROUP_HEIGHT = 400;

    private const int IG_GROUP_WIDTH = 150;
    private const int IG_GROUP_HEIGHT = 400;

    private void Update()
    {
        c = current;
        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
    }



    // ------------------------------------------------- OnGUI -------------------------------------------------
    private void OnGUI()
    {
        // PG Pre-Game Scene
        if (SceneManager.GetActiveScene().name == NetworkManager.singleton.offlineScene)
        {
            if (Menu.current == MenuPage.PG_Home)
                GUI_PG_Home();
            else if (Menu.current == MenuPage.PG_Listing)
                GUI_PG_Listing();
        }

        // IG In-Game Scene
        if (SceneManager.GetActiveScene().name == NetworkManager.singleton.onlineScene)
        {
            if (current != MenuPage.IG_Gameplay)
            {
                if (Menu.current == MenuPage.IG_ConfigureGame)
                    GUI_IG_ConfigureGame();
                else if (current == MenuPage.IG_LoadingMap)
                    GUI_IG_LoadingMap();
                else if (current == MenuPage.IG_Gameplay)
                    GUI_IG_Gameplay();
                else if (current == MenuPage.IG_MenuHome)
                    GUI_IG_MenuHome();
            }
        }

        GUIDebug();
    }













    // ------------------------------------------------- GUIHome -------------------------------------------------
    private void GUI_PG_Home()
    {
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
            current = MenuPage.IG_ConfigureGame;
            FindObjectOfType<Server>().CreateInternetMatch(Settings.PLAYER_NAME);
        }

        // Join Game
        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 60, 190, 120, 30), "Join") && Settings.IsValidName(Settings.PLAYER_NAME))
            Menu.current = MenuPage.PG_Listing;

        // Quit Game
        if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 60, 330, 120, 30), "Quit"))
            Application.Quit();

        GUI.EndGroup();
    }





    // ------------------------------------------------- GUIListing -------------------------------------------------
    private void GUI_PG_Listing()
    {
        GUI.skin = MenuSkin;
        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");


        if (GUI.Button(new Rect(20, PG_GROUP_HEIGHT - 50, 60, 30), "Back"))
            Back();
        GUI.Label(new Rect(PG_GROUP_WIDTH / 2 - 150, 40, 300, 30), "Hello [" + Settings.PLAYER_NAME + "]. Here are your matches");

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
                current = MenuPage.IG_ConfigureGame;
                FindObjectOfType<Server>().JoinInternetMatch(NetworkManager.singleton.matches[current_selection]);
            }

        GUI.EndGroup();
    }





    // ------------------------------------------------- GUIConfigureGame -------------------------------------------------
    private void GUI_IG_ConfigureGame()
    {
        GUI.skin = MenuSkin;
        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");


        if (GUI.Button(new Rect(20, PG_GROUP_HEIGHT - 50, 60, 30), "Back"))
            Back();
        

        GUI.Label(new Rect(0, 40, PG_GROUP_WIDTH / 3, 30), "Attacking");
        GUI.Label(new Rect(PG_GROUP_WIDTH * 2 / 3, 40, PG_GROUP_WIDTH / 3, 30), "Defending");

        if (Player.mine == null)
        {
            GUI.EndGroup();
            return;
        }

        Player[] players = FindObjectsOfType<Player>();
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
            GUI.Label(new Rect(0, 80 + 30 * i, PG_GROUP_WIDTH / 3, 30), attacking_players[i].player_name + " - " + attacking_players[i].possible_characters[attacking_players[i].selected_character].name);
        }
        if (Player.mine.selected_team != Team.A)
            if (GUI.Button(new Rect(20, 80 + 30 * attacking_players.Count, PG_GROUP_WIDTH / 3 - 40, 30), "Join Team"))
                Player.mine.CmdChangeSelectedTeam(Team.A);

        // Defending Players
        for (int i = 0; i < defending_players.Count; i++)
        {
            GUI.Label(new Rect(PG_GROUP_WIDTH * 2 / 3, 80 + 30 * i, PG_GROUP_WIDTH / 3, 30), defending_players[i].player_name + " - " + defending_players[i].possible_characters[defending_players[i].selected_character].name);
        }
        if (Player.mine.selected_team != Team.B)
            if (GUI.Button(new Rect(PG_GROUP_WIDTH * 2 / 3 + 20, 80 + 30 * defending_players.Count, PG_GROUP_WIDTH / 3 - 40, 30), "Join Team"))
                Player.mine.CmdChangeSelectedTeam(Team.B);

        for (int i = 0; i < neutral_players.Count; i++)
            GUI.Label(new Rect(PG_GROUP_WIDTH * 1 / 3, 80 + 30 * i, PG_GROUP_WIDTH / 3, 30), neutral_players[i].player_name);


        if (Player.mine.selected_team != Team.Neutral)
        {
            GUI.Label(new Rect(0, PG_GROUP_HEIGHT - 80, PG_GROUP_WIDTH, 30), "Selected Character");
            GUI.Label(new Rect(0, PG_GROUP_HEIGHT - 60, PG_GROUP_WIDTH, 30), Player.mine.possible_characters[Player.mine.selected_character].name);

            if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 - 60, PG_GROUP_HEIGHT - 60, 20, 20), "<"))
                Player.mine.CmdPreviousCharacter();
            if (GUI.Button(new Rect(PG_GROUP_WIDTH / 2 + 40, PG_GROUP_HEIGHT - 60, 20, 20), ">"))
                Player.mine.CmdNextCharacter();
        }

        if (Player.mine.is_host && neutral_players.Count == 0)
            if (GUI.Button(new Rect(PG_GROUP_WIDTH - 80, PG_GROUP_HEIGHT - 50, 60, 30), "Start"))
                StartCoroutine(FindObjectOfType<MapGenerator>().BeginGeneration());

        GUI.EndGroup();
    }






    // ------------------------------------------------- GUILoading -------------------------------------------------
    private void GUI_IG_LoadingMap()
    {
        GUI.skin = MenuSkin;
        GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, PG_GROUP_WIDTH, PG_GROUP_HEIGHT));
        GUI.Box(new Rect(0, 0, PG_GROUP_WIDTH, PG_GROUP_HEIGHT), "");

        GUI.Label(new Rect(0, PG_GROUP_HEIGHT / 2 - 10, PG_GROUP_WIDTH, 30), Settings.GetLoadingWords());

        GUI.EndGroup();
    }

    

    private void GUI_IG_Gameplay()
    {

    }

    private void GUI_IG_MenuHome()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MenuOverlay);
        GUI.skin = MenuSkin;
        
        GUI.BeginGroup(new Rect(20, Screen.height / 2 - IG_GROUP_HEIGHT / 2, IG_GROUP_WIDTH, IG_GROUP_HEIGHT)); // Screen.width / 2 - IG_GROUP_WIDTH / 2
        GUI.Box(new Rect(0, 0, IG_GROUP_WIDTH, IG_GROUP_HEIGHT), "");
        GUI.Label(new Rect(0, 0, IG_GROUP_WIDTH, IG_GROUP_HEIGHT), "menu", MenuSkin.FindStyle("MenuTitle"));

        if (Player.mine.is_host)
            if (GUI.Button(new Rect(IG_GROUP_WIDTH / 2 - 60, 50, 120, 30), "Swap Sides"))
            {
                FindObjectOfType<MapGenerator>().Reset();
                Back();
            }

        if (GUI.Button(new Rect(IG_GROUP_WIDTH / 2 - 60, 355, 120, 30), "Quit"))
            FindObjectOfType<Server>().ExitMatchToHome();
        GUI.EndGroup();
    }



    // ------------------------------------------------- Back -------------------------------------------------
    private void Back()
    {
        switch (current)
        {
            case MenuPage.PG_Home:
                {
                    break;
                }
            case MenuPage.PG_Listing:
                {
                    current_selection = 0;
                    Menu.current = MenuPage.PG_Home;
                    break;
                }
            case MenuPage.IG_ConfigureGame:
                {
                    if (Player.mine.is_host)
                        FindObjectOfType<Server>().DestroyInternetMatch();
                    FindObjectOfType<Server>().ExitMatchToHome();
                    break;
                }
            case MenuPage.IG_LoadingMap:
                {
                    break;
                }
            case MenuPage.IG_Gameplay:
                {
                    Menu.current = MenuPage.IG_MenuHome;
                    break;
                }
            case MenuPage.IG_MenuHome:
                {
                    Menu.current = MenuPage.IG_Gameplay;
                    break;
                }
        }
    }








    // ------------------------------------------------- GUIDebug -------------------------------------------------
    private void GUIDebug()
    {
        if (!Input.GetKey(KeyCode.RightControl))
            return;
        if (SceneManager.GetActiveScene().name == NetworkManager.singleton.offlineScene)
        {
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 20, 200, 20), "For debugging. No touchie!");
            if (GUI.Button(new Rect(Screen.width - 40, Screen.height - 20, 20, 20), "C"))
            {
                Settings.PLAYER_NAME = "host";
                current = MenuPage.IG_ConfigureGame;
                NetworkManager.singleton.StartHost();
            }
            if (GUI.Button(new Rect(Screen.width - 20, Screen.height - 20, 20, 20), "J"))
            {
                Settings.PLAYER_NAME = "client";
                NetworkManager.singleton.networkAddress = "localhost";
                current = MenuPage.IG_ConfigureGame;
                NetworkManager.singleton.StartClient();
            }
        }
    }
}
