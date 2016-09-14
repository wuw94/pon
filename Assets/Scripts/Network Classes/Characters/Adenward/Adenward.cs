using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class Adenward : Character
{
    // Characteristics
    public override float max_health { get { return 500; } set { throw new NotImplementedException(); } }
    public override float max_speed { get { return 3.5f; } set { throw new NotImplementedException(); } }

    // Shields
    public AdenwardShield shield;

    [SyncVar]
    public NetworkInstanceId id_shield = NetworkInstanceId.Invalid;

    [SyncVar]
    public bool show_shield;

    private float timer_shield;

    // Primary Weapon
    public AdenwardBash adenward_bash;
    private const float _primary_cooldown = 0.5f;






    // Skill 1 (Stronghold) LShift
    [SyncVar]
    public bool stronghold_mode = false;
    private const float _skill1_cooldown = 0.7f;

    // Skill 2 (Safeguard) Space
    private const float _skill2_cooldown = 1.0f;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(ShieldTimer());
        GameObject s = Instantiate<GameObject>(shield.gameObject);
        NetworkServer.Spawn(s);
        id_shield = s.GetComponent<NetworkIdentity>().netId;
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        ability_primary.SetCooldown(_primary_cooldown);
        ability_reload.SetCooldown(0);
        ability_skill1.SetCooldown(_skill1_cooldown);
        ability_skill1.name = "Stronghold";
        ability_skill2.SetCooldown(_skill2_cooldown);
        ability_skill2.name = "Safeguard";
        StartCoroutine(LookForShield());
    }

    public IEnumerator LookForShield()
    {
        while (id_shield == NetworkInstanceId.Invalid)
        {
            yield return null;
        }
        AdenwardShield s = ClientScene.FindLocalObject(id_shield).GetComponent<AdenwardShield>();
        s.transform.parent = this.transform;
        s.transform.localPosition = new Vector3(0, 0.34f, 0);
        s.transform.localRotation = Quaternion.Euler(0, 0, 0);
        shield = s;
        s.owner = this;
    }

    public IEnumerator ShieldTimer()
    {
        while (true)
        {
            if (isServer)
            {
                timer_shield -= Time.deltaTime;
                if (timer_shield <= 0)
                {
                    show_shield = true;
                    timer_shield = 0;
                }
                else
                    show_shield = false;
            }
            yield return null;
        }
    }


    public override void Passive()
    {
        if (stronghold_mode)
        {
            CmdInflictRoot(0.5f);
            CmdInflictLockPrimary(0.5f);
            CmdInflictLockSkill2(0.5f);
        }
    }

    // ------------------------------------------------- Bash -------------------------------------------------
    public override void PrimaryAttack()
    {
        ShakeCamera(0.02f, 0.05f, Quaternion.Euler(0, 0, GetMouseDirection(attacking_offset.position)));
        CmdSetShieldTimer();
        adenward_bash.ShowLocal(this.transform.position + transform.rotation * (Vector2.up * 0.5f), this.transform.rotation);
        CmdMakeAdenwardBash();
    }

    [Command]
    private void CmdSetShieldTimer()
    {
        timer_shield = 1;
    }

    [Command]
    private void CmdMakeAdenwardBash()
    {
        AdenwardBash ab = Instantiate<AdenwardBash>(adenward_bash);
        NetworkServer.SpawnWithClientAuthority(ab.gameObject, this.player.connectionToClient);
        ab.Make(this.transform.position + transform.rotation * (Vector2.up * 0.5f), this.transform.rotation, this.GetTeam(), this.player.connectionToClient);
    }

    // melee no reload lol
    public override void Reload()
    {
    }

    // ------------------------------------------------- Stronghold -------------------------------------------------
    public override void Skill1()
    {
        CmdSwitchMode();
    }
    
    [Command]
    private void CmdSwitchMode()
    {
        stronghold_mode = !stronghold_mode;
    }


    // ------------------------------------------------- Safeguard -------------------------------------------------
    public override void Skill2()
    {
        if (SA_rooted)
        {
            ability_skill2.Reset();
            return;
        }
        Character c = GetClosestAllyToMouse();
        if (c != null && Vector2.Distance(c.transform.position, this.transform.position) < 6)
        {
            Vector2 dash_to = c.transform.position;
            StartCoroutine(Safeguard(dash_to));
        }
        else
        {
            ability_skill2.Reset();
        }
    }

    private IEnumerator Safeguard(Vector2 dash_to)
    {
        float count = 0.3f;
        while (Vector2.Distance(this.transform.position, dash_to) > 1 && count > 0)
        {
            CmdInflictRoot(0.04f);
            Vector2 dir = dash_to - (Vector2)transform.position;
            GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(dir * 20, 20);
            count -= 0.02f;
            yield return new WaitForSeconds(0.02f);
        }

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }


    // ------------------------------------------------- GUI -------------------------------------------------
    protected override void OnGUI()
    {
        base.OnGUI();
        if (Player.mine.character != this)
            return;
        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 80, 300, 100), "Your Health: " + (int)GetHealth() + " / " + (int)max_health);
        GUI.Label(new Rect(30, Screen.height - 100, 300, 100), "Shield Health: " + (int)shield.GetHealth() + " / " + (int)shield.max_health);
        GUI.Label(new Rect(30, Screen.height - 80, 300, 100), ability_skill1.ToString());
        GUI.Label(new Rect(30, Screen.height - 60, 300, 100), ability_skill2.ToString());
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}