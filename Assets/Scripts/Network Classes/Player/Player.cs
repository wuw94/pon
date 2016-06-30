using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Player : NetworkEntity
{
    public static Player mine;
    
    [SyncVar]
    public bool on_nucleus = false;


    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void Dead()
    {
        StartCoroutine(RespawnProcess());
    }

    IEnumerator RespawnProcess()
    {
        yield return new WaitForSeconds(5);
        RpcPortToSpawn(GetTeam());
        Revive();
    }


    // Stuff to do just to a client player right when it loads
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Player.mine = this;
        //this.name = "Player " + this.connectionToClient.connectionId.ToString();
        AdjustLayer();
        LocalCamera();
        LocalVision();
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

    public void RefreshVision()
    {
        FindObjectOfType<DynamicLight>().Rebuild();
    }


    // -------------------------------------------------------  RPC  ------------------------------------------------------------

    

    [ClientRpc]
    public void RpcPortToSpawn(Team t)
    {
        if (!isLocalPlayer)
            return;
        
        Level level = FindObjectOfType<Level>();
        if (t == Team.A)
            transform.position = level.SpawnA + new Vector2(1, 1);
        if (t == Team.B)
            transform.position = level.SpawnB + new Vector2(1, 1);
    }
}
