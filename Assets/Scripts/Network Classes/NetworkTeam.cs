using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/* NetworkTeam.
 * 
 * Details:
 *  An abstract class that manages little details about a team. Objects on your team are tinted
 *  blue. Objects not on your team are tinted red. Your own objects have no tint at all (white).
 *  
 * 
 * Technicals:
 *  When we change teams, we want the change to occur in the server only, and only through SyncVars
 *  are we able to replicate the change onto our clients.
 *  
 *  Watch out for when your player changes his/her own team! When your own object changes teams,
 *  we have to redo the coloring.
 * 
 * auth Wesley Wu
 */
public abstract class NetworkTeam : NetworkBehaviour
{
    [SerializeField]
    [SyncVar(hook = "OnUpdateColor")] //[SyncVar(hook = "OnUpdateColor")]
    private Team _team = Team.Neutral; // What team this entity belongs to

    public readonly Color white = new Color(1, 1, 1, 1);
    public readonly Color blue = new Color(0.604f, 0.704f, 1, 1);
    public readonly Color red = new Color(1, 0.604f, 0.604f, 1);

    public virtual void Start()
    {
        ChangeTeam(_team);
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log(" " + red.r + " " + red.g + " " + red.b + " " + red.a);
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

    public void OnUpdateColor(Team t)
    {
        _team = t;
        if (isLocalPlayer) // Our own team changed and therefore we must update the colors of everything else.
            LocalUpdateColorAll();
        else
            UpdateColor();
    }

    private void LocalUpdateColorAll() // This is for when your own team changes.
    {
        foreach (NetworkTeam obj in FindObjectsOfType<NetworkTeam>())
            obj.UpdateColor();
    }

    protected virtual void UpdateColor()
    {
        if (hasAuthority)
            GetComponent<SpriteRenderer>().color = Color.cyan;
        else if (GetTeam() == Team.Neutral)
            GetComponent<SpriteRenderer>().color = Color.gray;
        else if (GetTeam() == Player.mine.GetTeam())
            GetComponent<SpriteRenderer>().color = blue;
        else if (GetTeam() != Player.mine.GetTeam())
            GetComponent<SpriteRenderer>().color = red;
        else
            GetComponent<SpriteRenderer>().color = Color.yellow;
    }
}
