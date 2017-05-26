using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/* CharacterInteractor.
 * 
 * Details:
 *  The CharacterInteractor has its own team and requires a collider.
 *  When a character enters a CharacterInteractor, the CharacterInteractor
 *  looks at the character's team, and adds it into one of two lists:
 *  
 *      allies_held :   Allies touching this collider
 *      enemies_held :  Enemies touching this collider
 *  
 * Technicals:
 *  I'm unsure whether OnTriggerEnter2D calls when a character is created
 *  directly on top of this CharacterInteractor.
 */

/// <summary>
/// A class for objects that produce effects on characters.
/// </summary>
public class CharacterInteractor : NetworkTeam
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
        if (!isServer)
            return;
        foreach (Character c in allies_held)
            DoToAlly(c);
        foreach (Character c in enemies_held)
            DoToEnemy(c);
    }

    /// <summary>
    /// Things to do to an ally character. Called once every Update().
    /// </summary>
    /// <param name="c"></param>
    public virtual void DoToAlly(Character c)
    {

    }

    /// <summary>
    /// Called when an ally enters this CharacterInteractor's collider.
    /// </summary>
    /// <param name="c"></param>
    public virtual void OnAllyEnter(Character c)
    {

    }

    /// <summary>
    /// Called when an enemy enters this CharacterInteractor's collider.
    /// </summary>
    /// <param name="c"></param>
    public virtual void OnEnemyEnter(Character c)
    {

    }

    /// <summary>
    /// Things to do to an enemy character. Called once every Update().
    /// </summary>
    /// <param name="c"></param>
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
            {
                if (!allies_held.Contains(c))
                {
                    allies_held.Add(c);
                    OnAllyEnter(c);
                }
            }
            else
            {
                if (!enemies_held.Contains(c))
                {
                    enemies_held.Add(c);
                    OnEnemyEnter(c);
                }
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
