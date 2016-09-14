using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/* NetworkTeam.
 * 
 * Details:
 *  All interactive objects that belong to a certain team derives from this class. NetworkTeam
 *  is an abstract class that manages little details about a team. Based on your static player
 *  object's team, objects on the same team as yours are tinted blue. Objects on the enemy team
 *  are tinted red. Your own objects are white. These colors do not have to be so, of course,
 *  so overridable functions are supplied:
 *  
 *  OnDisplayMine()
 *  OnDisplayNeutral()
 *  OnDisplayAlly()
 *  OnDisplayEnemy()
 *  
 * 
 * Technicals:
 *  We use the function UpdateColor(), which will update just this object's color. There are two
 *  main times when the color must be updated:
 *  
 *      1. Right when the object is created for the client
 *      2. Right when the object changes color
 *  
 *  We can tell when an object's team is changed through the convenient [SyncVar] attribute which
 *  sets a dirty bit to let clients know the value is changed. We do the updates inside a
 *  SyncVar 'hook' which calls only if the value changes. Changes to a team must only occur in the
 *  server.
 *  
 *  Note:
 *  When your own player object changes teams, every other object's relative coloring becomes
 *  different, and as thus, we need to change all their colors.
 * 
 * auth Wesley Wu
 */

/// <summary>
/// Class for managing an object's team.
/// </summary>
public abstract class NetworkTeam : NetworkBehaviour
{
    [SerializeField]
    [SyncVar(hook = "OnUpdateTeam")]
    private Team _team = Team.Neutral; // What team this entity belongs to

    public readonly Color white = new Color(1, 1, 1, 1);
    public readonly Color blue = new Color(0.304f, 0.304f, 1, 1);
    public readonly Color red = new Color(1, 0.304f, 0.304f, 1);


    public virtual void Start()
    {
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateColor();
        StartCoroutine(WaitForAuthority(20));
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    

    /// <summary>
    /// Returns the team this object belongs to.
    /// </summary>
    /// <returns></returns>
    public Team GetTeam()
    {
        return _team;
    }

    /// <summary>
    /// Change this object's team.
    /// </summary>
    /// <param name="t"></param>
    public void ChangeTeam(Team t)
    {
        if (!isServer)
            return;
        _team = t;
    }

    /// <summary>
    /// Change this object's team (only called on prefabs before spawning)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="call_on_prefab"></param>
    public void PreSpawnChangeTeam(Team t)
    {
        _team = t;
    }

    /// <summary>
    /// Hook for the [SyncVar] _team. We want to update color, but also if our own team changes,
    /// we must update the colors of everything else because their relative colors have changed.
    /// </summary>
    /// <param name="t"></param>
    protected virtual void OnUpdateTeam(Team t)
    {
        _team = t;
        if (this == Player.mine.character) // Our own team changed and therefore we must update the colors of everything else.
            LocalUpdateColorAll();
        else
            UpdateColor();
        OnTeamChanged();
    }

    /// <summary>
    /// Update the colors of all objects. Only call this on local player.
    /// </summary>
    private void LocalUpdateColorAll() // This is for when your own team changes.
    {
        if (this != Player.mine.character)
            Debug.LogWarning("You're updating colors of all objects, but not for the local player. Beware, this is a costly function");
        foreach (NetworkTeam obj in FindObjectsOfType<NetworkTeam>())
            obj.UpdateColor();
    }

    private void UpdateColor()
    {
        if (Player.mine == null)
            return;
        if (GetTeam() == Team.Neutral)
            OnDisplayNeutral();
        else if (GetTeam() == Player.mine.selected_team)
            OnDisplayAlly();
        else
            OnDisplayEnemy();
        if (hasAuthority)
            OnDisplayMine();
    }


    /// <summary>
    /// Wait for a number of frames for authority to be assigned, since
    /// authority isn't assigned immediately upon spawn
    /// </summary>
    /// <param name="frames"></param>
    /// <returns></returns>
    public IEnumerator WaitForAuthority(int frames)
    {
        int count = 0;
        while (true)
        {
            count++;
            if (hasAuthority)
            {
                UpdateColor();
                break;
            }
            if (count > frames)
                break;
            yield return null;
        }
    }

    /// <summary>
    /// Called when this object's team changes. Called after your color is
    /// updated.
    /// </summary>
    protected virtual void OnTeamChanged()
    {
    }

    /// <summary>
    /// How to augument the display if this object is mine.
    /// </summary>
    protected virtual void OnDisplayMine()
    {
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().color = new Color(white.r, white.g, white.b, GetComponent<SpriteRenderer>().color.a);
    }

    /// <summary>
    /// How to augument the display if this object is neutral.
    /// </summary>
    protected virtual void OnDisplayNeutral()
    {
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, GetComponent<SpriteRenderer>().color.a);
    }

    /// <summary>
    /// How to augument the display if this object is my ally.
    /// </summary>
    protected virtual void OnDisplayAlly()
    {
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().color = new Color(blue.r, blue.g, blue.b, GetComponent<SpriteRenderer>().color.a);
    }

    /// <summary>
    /// How to augument the display if this object is my enemy.
    /// </summary>
    protected virtual void OnDisplayEnemy()
    {
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().color = new Color(red.r, red.g, red.b, GetComponent<SpriteRenderer>().color.a);
    }

}
