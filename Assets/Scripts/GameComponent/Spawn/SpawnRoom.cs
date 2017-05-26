using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SpawnRoom : NetworkTeam
{
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.GetComponent<Character>() != null)
        {
            if (col.GetComponent<Character>().GetTeam() == this.GetTeam())
                col.GetComponent<Character>().ChangeHealth(null, 200 * Time.deltaTime);
            else
                col.GetComponent<Character>().ChangeHealth(null, -200 * Time.deltaTime);
        }
    }

    protected override void OnDisplayMine()
    {
    }
}
