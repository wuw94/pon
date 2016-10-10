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
    public AdenwardBashLogic adenward_bash_logic;
    public AdenwardBashView adenward_bash_view;
    private const float _primary_cooldown = 0.5f;






    // Skill 1 (Stronghold) LShift
    [SyncVar]
    public bool stronghold_mode = false;
    private const float _skill1_cooldown = 0.7f;

    // Skill 2 (Safeguard) Space
    private const float _skill2_cooldown = 7.0f;
    private const float SAFEGUARD_RANGE_MAX = 4.0f;
    private const float SAFEGUARD_RANGE_MIN = 1.5f;
    private GameObject adenward_dash_to_image;

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

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        adenward_dash_to_image = Instantiate(Resources.Load<GameObject>("Characters/Adenward/AdenwardDashTo"));
    }

    public override void Update()
    {
        base.Update();
        if (hasAuthority)
            ManageDashToImage();
    }

    private void ManageDashToImage()
    {
        Character c = GetClosestAllyToMouse();
        if (c != null && Vector2.Distance(c.transform.position, this.transform.position) < SAFEGUARD_RANGE_MAX && Vector2.Distance(c.transform.position, this.transform.position) > SAFEGUARD_RANGE_MIN)
        {
            adenward_dash_to_image.transform.position = c.transform.position;
            adenward_dash_to_image.transform.rotation = Face(adenward_dash_to_image.transform.position, transform.position);
            adenward_dash_to_image.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.6f, 1, Mathf.Lerp(adenward_dash_to_image.GetComponent<SpriteRenderer>().color.a, 0.5f, 20 * Time.deltaTime));
        }
        else
            adenward_dash_to_image.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(adenward_dash_to_image.GetComponent<SpriteRenderer>().color.a, 0, 20 * Time.deltaTime));
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
        if (SA_stunned)
        {
            stronghold_mode = false;
            timer_shield = 1;
        }
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
        AdenwardBashView abv = Instantiate(adenward_bash_view);
        abv.transform.position = this.transform.position + transform.rotation * (Vector2.up * 0.5f);
        abv.transform.rotation = this.transform.rotation;
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
        AdenwardBashLogic abl = Instantiate(adenward_bash_logic);
        abl.transform.position = this.transform.position + transform.rotation * (Vector2.up * 0.5f);
        abl.transform.rotation = this.transform.rotation;

        abl.owner_id = netId;
        abl.PreSpawnChangeTeam(GetTeam());
        NetworkServer.Spawn(abl.gameObject);
        RpcMakeAdenwardBash();
    }

    [ClientRpc]
    private void RpcMakeAdenwardBash()
    {
        if (player == Player.mine)
            return;
        AdenwardBashView abv = Instantiate(adenward_bash_view);
        abv.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        abv.transform.position = this.transform.position + transform.rotation * (Vector2.up * 0.5f);
        abv.transform.rotation = this.transform.rotation;
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
        if (c != null && Vector2.Distance(c.transform.position, this.transform.position) < SAFEGUARD_RANGE_MAX)
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
        if (stronghold_mode)
            GUI.Label(new Rect(30, Screen.height - 80, 300, 100), "Stronghold: [ON]");
        else
            GUI.Label(new Rect(30, Screen.height - 80, 300, 100), "Stronghold: [OFF]");
        GUI.Label(new Rect(30, Screen.height - 60, 300, 100), ability_skill2.ToString());
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        if (adenward_dash_to_image != null)
            Destroy(adenward_dash_to_image.gameObject);
    }
}