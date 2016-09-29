using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SpawnRoom : NetworkTeam
{
    List<Character> allies = new List<Character>();
    List<Character> enemies = new List<Character>();

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
            {
                col.GetComponent<Character>().ChangeHealth(200 * Time.deltaTime);
            }
            else
            {
                col.GetComponent<Character>().ChangeHealth(-200 * Time.deltaTime);
            }
        }
    }

    protected override void OnDisplayMine()
    {
    }
}
