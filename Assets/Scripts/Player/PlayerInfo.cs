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

    public const int max_health = 100;
    public bool display_name = false;


    [SyncVar]
    public int current_health = max_health;

    // a bool to check if we're currently standing on a nucleus
    [SyncVar]
    public bool on_nucleus = false;


    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }

        current_health -= amount;
        if (current_health <= 0)
        {
            current_health = max_health;
            RpcPortToSpawn();
        }

    }




    // Stuff to do just to a client player (like changing its color so you can easily recognize it)
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        //this.name = "Player " + this.connectionToClient.connectionId.ToString();
        AdjustLayer();
        LocalCamera();
        LocalVision();
    }



    private void Start()
    {
        if (!isLocalPlayer)
            return;
        
    }

    private void AdjustLayer()
    {
        this.gameObject.layer = 5;
        this.gameObject.GetComponentInChildren<Health>().gameObject.layer = 5;
    }

    private void LocalCamera()
    {
        Instantiate(Resources.Load<GameObject>("Camera/Camera (View Under)"));
        Camera.main.GetComponent<LerpFollow>().target = this.transform;
    }

    private void LocalVision()
    {
        GameObject g = Instantiate(Resources.Load<GameObject>("Player/Vision"));
        g.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, g.transform.position.z);
        g.transform.rotation = this.transform.rotation;
        g.transform.parent = this.transform;
    }


    private void Update()
    {
        if (!isLocalPlayer)
            return;
        UpdateColor();
        if (Input.GetKeyDown(KeyCode.M))
            RefreshVision();
    }

    public void RefreshVision()
    {
        FindObjectOfType<DynamicLight>().Rebuild();
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
