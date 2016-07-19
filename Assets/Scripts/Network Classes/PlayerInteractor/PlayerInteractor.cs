using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerInteractor : NetworkTeam
{
    /// <summary>
    /// Allies inside the PlayerInteractor's collider trigger
    /// </summary>
    protected List<Character> allies_held = new List<Character>();

    /// <summary>
    /// Enemies inside the PlayerInteractor's collider trigger
    /// </summary>
    protected List<Character> enemies_held = new List<Character>();

    public override void Update()
    {
        foreach (Character c in allies_held)
            DoToAlly(c);
        foreach (Character c in enemies_held)
            DoToEnemy(c);
    }

    public virtual void DoToAlly(Character c)
    {

    }

    public virtual void DoToEnemy(Character c)
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (this.GetTeam() == Team.Neutral)
            return;
        if (col.GetComponent<Character>() != null)
        {
            
            Character c = col.GetComponent<Character>();
            if (c.GetTeam() == this.GetTeam())
                allies_held.Add(c);
            else
            {
                enemies_held.Add(c);
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (this.GetTeam() == Team.Neutral)
            return;
        if (col.GetComponent<Character>() != null)
        {
            Character c = col.GetComponent<Character>();
            if (c.GetTeam() == this.GetTeam())
                allies_held.Remove(c);
            else
                enemies_held.Remove(c);
        }
    }
    
}
