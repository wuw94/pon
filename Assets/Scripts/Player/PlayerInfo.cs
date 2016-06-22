using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public enum Team
{
    A,B,Neutral
}

public class PlayerInfo : NetworkBehaviour
{
    // For GUI
    public int[] connections = new int[10];
    public int num_connections = 0;
    public int networkID = 0;

    [SyncVar]
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
            currentHealth = maxHealth;
            RpcPortToSpawn();
        }

    }




    // Stuff to do just to a client player (like changing its color so you can easily recognize it)
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        //this.name = "Player " + this.connectionToClient.connectionId.ToString();
        Instantiate(Resources.Load<GameObject>("Camera/Camera"));
        Camera.main.GetComponent<LerpFollow>().target = this.transform;

    }



    private void Start()
    {
        if (!isLocalPlayer)
            return;
    }




    private void Update()
    {
        if (!isLocalPlayer)
            return;
        UpdateColor();
    }


    

    private void UpdateColor()
    {
        PlayerInfo[] p = FindObjectsOfType<PlayerInfo>();
        for (int i = 0; i < p.Length; i++)
        {
            if (p[i].team == Team.Neutral)
                p[i].GetComponent<Renderer>().material.color = Color.gray;
            else if (p[i].team != this.team)
                p[i].GetComponent<Renderer>().material.color = Color.red;
            else
                p[i].GetComponent<Renderer>().material.color = Color.blue;
        }
        GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnMouseEnter()
    {
        display_name = true;
    }

    private void OnMouseExit()
    {
        display_name = false;
    }



    // -------------------------------------------------------  RPC  ------------------------------------------------------------

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
    public void RpcUpdateTeam(Team t)
    {
        team = t;
    }

    [ClientRpc]
    public void RpcPortToSpawn()
    {
        if (!isLocalPlayer)
            return;
        Level level = FindObjectOfType<Level>();
        if (team == Team.A)
            transform.position = level.SpawnA + new Vector2(1, 1);
        if (team == Team.B)
            transform.position = level.SpawnB + new Vector2(1, 1);
    }


}
