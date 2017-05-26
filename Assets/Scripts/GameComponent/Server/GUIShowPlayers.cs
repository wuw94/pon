using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GUIShowPlayers : NetworkBehaviour
{
    // For GUI
    public int[] connections = new int[10];
    public int num_connections = 0;
    public int networkID = 0;

    private void OnGUI()
    {
        if (isLocalPlayer && Input.GetKey(KeyCode.Tab))
        {
            GUI.Label(new Rect(0, 200, 100, 20), "Connections: " + num_connections);
            for (int i = 0; i < 10; i++)
                if (connections[i] != -1)
                {
                    GUI.Label(new Rect(0, 220 + 20 * i, 300, 20), "  Player " + connections[i].ToString() + "");
                }
                else
                {
                    GUI.Label(new Rect(0, 220 + 20 * i, 300, 20), "------ No Player ------");
                }
            GUI.Label(new Rect(80, 220 + 20 * networkID, 300, 20), "(You)");

        }
    }

    [ClientRpc]
    public void RpcUpdateConnections(int[] c, int id)
    {
        connections = c;
        int count = 0;
        for (int i = 0; i < c.Length; i++)
            if (c[i] != -1)
                count++;
        num_connections = count;
        networkID = id;
    }
}
