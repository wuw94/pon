using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class SpawnRoom : NetworkTeam
{
    List<Character> allies = new List<Character>();
    List<Character> enemies = new List<Character>();

    public override void Update()
    {
        if (!isServer)
            return;
        foreach (Character ally in allies)
            ally.ChangeHealth(50 * Time.deltaTime);
        foreach (Character enemy in enemies)
            enemy.ChangeHealth(-50 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.GetComponent<Character>() != null)
        {
            if (col.GetComponent<Character>().GetTeam() == this.GetTeam())
                allies.Add(col.GetComponent<Character>());
            else
                enemies.Add(col.GetComponent<Character>());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.GetComponent<Character>() != null)
        {
            if (col.GetComponent<Character>().GetTeam() == this.GetTeam())
                allies.Remove(col.GetComponent<Character>());
            else
                enemies.Remove(col.GetComponent<Character>());
        }
    }

    protected override void OnDisplayMine()
    {
    }
}
