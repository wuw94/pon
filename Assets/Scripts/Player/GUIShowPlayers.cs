using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GUIShowPlayers : NetworkBehaviour
{
    public PlayerInfo player;

    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            foreach (PlayerInfo p in FindObjectsOfType<PlayerInfo>())
            {
                if (!p.display_name)
                    break;
                GUI.Label(new Rect(Camera.main.WorldToScreenPoint(p.transform.position).x,
                                Camera.main.WorldToScreenPoint(p.transform.position).y,
                                200, 20),
                                p.name);
            }


            GUI.Label(new Rect(0, 200, 100, 20), "Connections: " + player.num_connections);
            for (int i = 0; i < 10; i++)
                if (player.connections[i] != -1)
                {
                    GUI.Label(new Rect(0, 220 + 20 * i, 300, 20), "  Player " + player.connections[i].ToString() + "");
                }
                else
                {
                    GUI.Label(new Rect(0, 220 + 20 * i, 300, 20), "------ No Player ------");
                }
            GUI.Label(new Rect(80, 220 + 20 * player.networkID, 300, 20), "(You)");
        }
    }
}
