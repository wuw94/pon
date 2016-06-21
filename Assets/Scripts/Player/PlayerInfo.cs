using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public enum Team
{
    A,B
}

public class PlayerInfo : NetworkBehaviour
{
    // [Command] called on client, executed on server, important for stuff that's client generated
    // [ClientRpc] called on server, executed on client, important for controlling client because clients have authority
    // [SyncVar] not shared, but your changes are reflected on everybody's computer (for just your object).

    // For GUI
    public int[] connections = new int[10];
    public int num_connections = 0;
    public int networkID = 0;
    public Team team;

    public const int maxHealth = 100;
    public bool display_name = false;


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
        if (!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            CmdPrintOnServer("Hello from: ");
        }
        

        UpdateColor();
    }

    

    private void UpdateColor()
    {
        if (isLocalPlayer)
        {
            PlayerInfo[] p = FindObjectsOfType<PlayerInfo>();
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].team != this.team)
                    p[i].GetComponent<Renderer>().material.color = Color.red;
                else
                    p[i].GetComponent<Renderer>().material.color = Color.blue;
            }
            GetComponent<Renderer>().material.color = Color.white;
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

    [ClientRpc]
    public void RpcUpdateTeam(int id)
    {
        team = id % 2 == 0 ? Team.A : Team.B;
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
