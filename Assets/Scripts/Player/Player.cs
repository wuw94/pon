using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : Controllable
{
    //[Command] called on client, executed on server, important for stuff that's client generated
    //[ClientRpc] called on server, executed on client, important for controlling client because clients have authority


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


    public override void Start() {
        base.Start();
	}
	
	// Update is called once per frame
	public override void FixedUpdate() {
        base.FixedUpdate();
	}

    // Stuff to do just to a client player (like changing its color so you can easily recognize it)
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdUpdatePlayerID();
        Instantiate(Resources.Load<GameObject>("Camera/Camera"));
        Camera.main.GetComponent<LerpFollow>().target = this.transform;

    }

    

    [Command]
    public void CmdUpdatePlayerID()
    {
        transform.name = "Player [id:" + netId.ToString() + "]";
    }

    [Command]
    public void CmdPrintOnServer(string message)
    {
        Debug.Log(message);
    }
}
