using UnityEngine;
using UnityEngine.Networking;

/* NetworkEntity. (TODO: Finish writing this)
 * 
 * Details:
 *  Abstract class deriving from NetworkTeam.
 *  
 * 
 * Technicals:
 *  ChangeAlpha for when an entity dies.
 * 
 * auth Wesley Wu
 */
public abstract class NetworkEntity : NetworkTeam
{
    public abstract float max_health { get; set; }

    [SyncVar]
    private float _current_health = 100;

    [SyncVar(hook = "OnDead")]
    private bool _is_dead = false;

    public float GetHealth()
    {
        return _current_health;
    }

    public bool IsDead()
    {
        return _is_dead;
    }
    
    public void ChangeHealth(float amount)
    {
        if (!isServer) // Changes to an entity's health should only be executed on the server
            return;

        if (_current_health <= 0 || _is_dead)
            return;

        _current_health += amount;

        if (_current_health > max_health)
        {
            _current_health = max_health;
        }

        if (_current_health <= 0)
        {
            _current_health = 0;
            _is_dead = true;
            Dead();
        }
    }

    private void OnDead(bool d)
    {
        _is_dead = d;
        Color c = GetComponent<SpriteRenderer>().color;
        if (_is_dead)
            GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0.2f);
        else
            GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 1);
    }

    public void Revive()
    {
        if (!isServer) // Changes to an entity's health should only be executed on the server
            return;

        _current_health = max_health;
        _is_dead = false;
    }

    public abstract void Dead();
}