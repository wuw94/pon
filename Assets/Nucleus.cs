using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Nucleus : NetworkBehaviour
{
    public Color co;
    [SyncVar]
    public Team team;

    public const int max_health = 100;

    [SyncVar]
    public float current_health = max_health;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerInfo>().on_nucleus = true;
            if (col.GetComponent<PlayerInfo>().team == this.team)
            {
                ChangeHealth(1);
            }
            else
            {
                ChangeHealth(-1);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerInfo>().on_nucleus = false;
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
        Color c = new Color(1, 1, 1, current_health/100);
        GetComponent<Renderer>().material.color = c;
    }

    [Command]
    public void CmdPrintOnServer(string message)
    {
        Debug.Log(message);
    }
}
