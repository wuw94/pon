using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : Controllable
{
    // [Command] called on client, executed on server, important for stuff that's client generated
    // [ClientRpc] called on server, executed on client, important for controlling client because clients have authority
    // [SyncVar] not shared, but your changes are reflected on everybody's computer (for just your object).

    // For GUI
    public int[] connections = new int[10];
    public int num_connections = 0;
    public int networkID = 0;

    public bool display_name = false;

    public const int maxHealth = 100;

    [SyncVar]
    public int currentHealth = maxHealth;


    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Dead!");
        }

    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(networkID);
        }
    }

    private void OnMouseEnter()
    {
        display_name = true;
    }

    private void OnMouseExit()
    {
        display_name = false;
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

    private void OnGUI()
    {
        

        if (isLocalPlayer)
        {
            foreach (Player p in FindObjectsOfType<Player>())
            {
                if (!p.display_name)
                    break;
                GUI.Label(new Rect(Camera.main.WorldToScreenPoint(p.transform.position).x,
                                Camera.main.WorldToScreenPoint(p.transform.position).y,
                                200, 20),
                                p.name);
            }


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
            GUI.Label(new Rect(80, 220 + 20 * this.networkID, 300, 20), "(You)");
        }
        
    }


    // Stuff to do just to a client player (like changing its color so you can easily recognize it)
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        //this.name = "Player " + this.connectionToClient.connectionId.ToString();
        CmdUpdatePlayerID(this.name);
        Instantiate(Resources.Load<GameObject>("Camera/Camera"));
        Camera.main.GetComponent<LerpFollow>().target = this.transform;
    }

    [Command]
    public void CmdUpdatePlayerID(string name)
    {
        transform.name = name;
        RpcUpdatePlayerID(name);
    }

    [ClientRpc]
    public void RpcUpdatePlayerID(string name)
    {
        transform.name = name;
    }

    /// <summary>
    /// Use this to print something on the server's debug. Useful if we want to debug stuff
    /// </summary>
    /// <param name="message"></param>
    [Command]
    public void CmdPrintOnServer(string message)
    {
        Debug.Log(message);
    }
}
