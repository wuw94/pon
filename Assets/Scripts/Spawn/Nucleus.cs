using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Nucleus : NetworkTeam
{
    private List<Character> _dismantling = new List<Character>();
    private List<Character> _repairing = new List<Character>();

    public const int max_health = 100;

    public Sprite dead_image;

    [SyncVar]
    private float current_health = max_health;

    public void OnGUI()
    {
        if (!isClient)
            return;
        if (Player.mine == null)
            return;
        if (Player.mine.character == null)
            return;
        if (Player.mine.character.GetTeam() == Team.Neutral)
            return;
        if (Player.mine.character.GetTeam() == this.GetTeam())
        {
            string ally_text = "Your Nucleus: " + (int)(current_health / max_health * 100) + "%";
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 40, 300, 100), ally_text);

        }
        else
        {
            string enemy_text = "Enemy Nucleus: " + (int)(current_health / max_health * 100) + "%";
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 40, 300, 100), enemy_text);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.GetComponent<Character>() != null)
        {
            if (col.GetComponent<Character>().GetTeam() == this.GetTeam())
            {
                _repairing.Add(col.GetComponent<Character>());
            }
            else
            {
                _dismantling.Add(col.GetComponent<Character>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.GetComponent<Character>() != null)
        {
            if (col.GetComponent<Character>().GetTeam() == this.GetTeam())
            {
                if (_repairing.Contains(col.GetComponent<Character>()))
                    _repairing.Remove(col.GetComponent<Character>());
            }
            else
            {
                if (_dismantling.Contains(col.GetComponent<Character>()))
                    _dismantling.Remove(col.GetComponent<Character>());
            }
        }
    }


    public void ChangeHealth(float amount)
    {
        current_health += amount;
        if (current_health > max_health)
            current_health = max_health;
        if (current_health <= 0)
            HealthZero();
    }

    private void HealthZero()
    {
        RpcGameEnd(GetTeam());
        GetComponent<SpriteRenderer>().sprite = dead_image;
    }

    public override void Update()
    {
        base.Update();
        UpdateHealth();
        GetComponent<SpriteRenderer>().color = new Color(   GetComponent<SpriteRenderer>().color.r,
                                                            GetComponent<SpriteRenderer>().color.g,
                                                            GetComponent<SpriteRenderer>().color.b,
                                                            current_health / max_health);
        RemoveDead();
    }

    private void RemoveDead()
    {
        foreach (Character c in _repairing)
            if (c.IsDead())
            {
                _repairing.Remove(c);
                break;
            }
        foreach (Character c in _dismantling)
            if (c.IsDead())
            {
                _dismantling.Remove(c);
                break;
            }
    }

    private void UpdateHealth()
    {
        if (!isServer)
            return;
        if (_repairing.Count == 0)
            ChangeHealth(-_dismantling.Count * 10 * Time.deltaTime);
        if (_dismantling.Count == 0)
            ChangeHealth(_repairing.Count * 10 * Time.deltaTime);
    }

    protected override void OnDisplayMine()
    {
        
    }

    [ClientRpc]
    private void RpcGameEnd(Team loser)
    {
        if (loser == Player.mine.character.GetTeam())
            Debug.LogError("Your team lost.  :(");
        else
            Debug.LogError("Your team won! Who woulda thunk it");
        Destroy(this.gameObject);
    }

    [Command]
    public void CmdPrintOnServer(string message)
    {
        Debug.Log(message);
    }
}
