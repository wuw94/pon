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

/// <summary>
/// Class for managing entities. Entities are objects that have health and can die.
/// </summary>
public abstract class NetworkEntity : NetworkTeam
{
    public abstract float max_health { get; set; }

    [SyncVar]
    private float _current_health = 100;

    private float _current_health_lerp = 0;

    /// <summary>
    /// should only be accessed from the server.
    /// </summary>
    protected float time_of_recent_damage;

    [SyncVar(hook = "OnDead")]
    private bool _is_dead = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
        time_of_recent_damage = Time.time;
    }

    public override void Update()
    {
        base.Update();
        LerpHealth();
    }

    private void LerpHealth()
    {
        _current_health_lerp = Mathf.Lerp(_current_health_lerp, _current_health, Time.deltaTime * 10);
        if (_current_health == 0 && _current_health_lerp < 1)
            _current_health_lerp = 0;
    }


    public float GetHealthLerp()
    {
        return _current_health_lerp;
    }

    public float GetHealth()
    {
        return _current_health;
    }

    public bool IsDead()
    {
        return _is_dead;
    }
    
    public void ChangeHealth(Player source, float amount)
    {
        if (!isServer) // Changes to an entity's health should only be executed on the server
        {
            // If ChangeHealth is called on somebody other than the server, that means we want to show it locally.
            _current_health += amount;
            if (_current_health > max_health)
                _current_health = max_health;
            if (_current_health <= 0)
                _current_health = 0;
            return;
        }

        if (_current_health <= 0 || _is_dead)
            return;

        if (amount < 0)
            time_of_recent_damage = Time.time;

        _current_health += amount; // the change

        if (_current_health > max_health)
            _current_health = max_health;

        if (_current_health <= 0)
        {
            _current_health = 0;
            _is_dead = true;
            Dead(source);
        }
    }

    private void OnDead(bool d)
    {
        _is_dead = d;
        Color c = GetComponent<SpriteRenderer>().color;
        if (_is_dead)
        {
            GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0.2f);
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
            foreach (Collider2D c2d in GetComponents<Collider2D>())
                c2d.enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 1);
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
            foreach (Collider2D c2d in GetComponents<Collider2D>())
                c2d.enabled = true;
        }
    }

    public void Revive()
    {
        if (!isServer) // Changes to an entity's health should only be executed on the server
            return;

        _current_health = max_health;
        _is_dead = false;
    }

    public abstract void Dead(Player source);
}