using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerInteractor : NetworkTeam
{
    /// <summary>
    /// Allies inside the PlayerInteractor's collider trigger
    /// </summary>
    protected List<Player> allies_held = new List<Player>();

    /// <summary>
    /// Enemies inside the PlayerInteractor's collider trigger
    /// </summary>
    protected List<Player> enemies_held = new List<Player>();

    public override void Update()
    {
        foreach (Player p in allies_held)
            DoToAlly(p);
        foreach (Player p in enemies_held)
            DoToEnemy(p);
    }

    public virtual void DoToAlly(Player p)
    {

    }

    public virtual void DoToEnemy(Player p)
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (this.GetTeam() == Team.Neutral)
            return;
        if (col.gameObject.tag == "Player")
        {
            
            Player p = col.gameObject.GetComponent<Player>();
            if (p.GetTeam() == this.GetTeam())
                allies_held.Add(p);
            else
            {
                enemies_held.Add(p);
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (this.GetTeam() == Team.Neutral)
            return;
        if (col.gameObject.tag == "Player")
        {
            Player p = col.gameObject.GetComponent<Player>();
            if (p.GetTeam() == this.GetTeam())
                allies_held.Remove(p);
            else
                enemies_held.Remove(p);
        }
    }
    
}
