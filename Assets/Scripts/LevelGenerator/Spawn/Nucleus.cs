using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Nucleus : NetworkBehaviour
{
    public Color co;
    private int num_dismantling = 0;
    private int num_repairing = 0;

    [SyncVar]
    public Team team;

    public const int max_health = 100;

    [SyncVar]
    public float current_health = max_health;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.GetComponent<Character>() != null)
        {
            //col.GetComponent<Player>().on_nucleus = true;
            if (col.GetComponent<Character>().GetTeam() == this.team)
            {
                num_repairing++;
            }
            else
            {
                num_dismantling++;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.GetComponent<Character>() != null)
        {
            //col.GetComponent<Player>().on_nucleus = false;
            if (col.GetComponent<Character>().GetTeam() == this.team)
            {
                num_repairing--;
            }
            else
            {
                num_dismantling--;
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
        Destroy(this.gameObject);
    }

    private void Update()
    {
        UpdateHealth();
        Color c = new Color(1, 1, 1, current_health/100);
        GetComponent<Renderer>().material.color = c;
    }

    private void UpdateHealth()
    {
        if (!isServer)
            return;
        ChangeHealth(num_repairing - num_dismantling);
    }

    [Command]
    public void CmdPrintOnServer(string message)
    {
        Debug.Log(message);
    }
}
