using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Nucleus : NetworkTeam
{
    public List<Character> _dismantling = new List<Character>();
    public List<Character> _repairing = new List<Character>();

    public const int max_health = 100;

    public Sprite dead_image;

    [SyncVar]
    private float current_health = max_health;

    private float lerp_health = max_health;


    // GUI Stuff
    public Texture2D hud;
    public Texture2D bar;
    public Texture2D[] glow;
    public int glow_int = 0;

    [SyncVar]
    public bool show_glow = false;

    public GUISkin bar_skin;

    

    public override void Start()
    {
        base.Start();
        StartCoroutine(loopglow());
    }


    private IEnumerator loopglow()
    {
        while (true)
        {
            if (isServer)
                show_glow = _dismantling.Count + _repairing.Count > 0 && current_health != max_health;

            glow_int++;
            if (glow_int >= glow.Length)
                glow_int = 0;
            yield return new WaitForSeconds(0.05f);
        }
    }

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
            GUI.skin = bar_skin;
            GUI.DrawTexture(new Rect(Screen.width / 2 - hud.width / 2, 0, hud.width, hud.height), hud);

            GUI.Box(new Rect(Screen.width / 2 - 135, 0, 20 + 250 * lerp_health / max_health, bar.height), "");
            if (show_glow)
                GUI.DrawTexture(new Rect(Screen.width / 2 - hud.width / 2 + 50 + 250 * lerp_health / max_health, 0, glow[glow_int].width, glow[glow_int].height), glow[glow_int]);
            GUI.Label(new Rect(Screen.width / 2 - 150, 3, 300, 30), "Your Nucleus");
        }
        else
        {
            GUI.skin = bar_skin;
            GUI.DrawTexture(new Rect(Screen.width / 2 - hud.width / 2, 0, hud.width, hud.height), hud);

            GUI.Box(new Rect(Screen.width / 2 - 135, 0, 20 + 250 * lerp_health / max_health, bar.height), "");
            if (show_glow)
                GUI.DrawTexture(new Rect(Screen.width / 2 - hud.width / 2 + 50 + 250 * lerp_health / max_health, 0, glow[glow_int].width, glow[glow_int].height), glow[glow_int]);
            GUI.Label(new Rect(Screen.width / 2 - 150, 3, 300, 30), "Enemy Nucleus");
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
                if (!_repairing.Contains(col.GetComponent<Character>()))
                    _repairing.Add(col.GetComponent<Character>());
            }
            else
            {
                if (!_dismantling.Contains(col.GetComponent<Character>()))
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
        lerp_health = Mathf.Lerp(lerp_health, current_health, Time.deltaTime * 20);
        RemoveDead();
    }

    private void RemoveDead()
    {
        foreach (Character c in _repairing)
            if (c.IsDead() || c == null)
            {
                _repairing.Remove(c);
                break;
            }
        foreach (Character c in _dismantling)
            if (c.IsDead() || c == null)
            {
                _dismantling.Remove(c);
                break;
            }
    }

    private void UpdateHealth()
    {
        if (!isServer)
            return;
        if (_repairing.Count == 0 && _dismantling.Count > 0)
            ChangeHealth(-5 * Time.deltaTime);
        if (_dismantling.Count == 0 && _repairing.Count > 0)
            ChangeHealth(5 * Time.deltaTime);
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
