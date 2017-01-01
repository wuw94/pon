using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class Kam : Character
{
    // Characteristics
    public override float max_health { get { return 200; } set { throw new NotImplementedException(); } }
    public override float max_speed { get { return 3.6f; } set { throw new NotImplementedException(); } }

    // Passive
    private float PASSIVE_MULTIPLIER = 0.8f;

    // Primary Weapon
    public KamSlashLogic[] kam_slash_logic;
    public KamSlashView[] kam_slash_view;
    private int slash_num = 0;
    private float slash_timeout = 0;
    private bool mouse_up = true;
    private const float _primary_cooldown = 0.1f;

    // Skill 1 (Tempest) LShift
    private const string TEMPEST_NAME = "KAM_TEMPEST";
    private const float TEMPEST_MOVEMENT_MAGNITUDE = 0.5f;
    private const float TEMPEST_DURATION = 1.0f;
    private const float TEMPEST_WIND_SLASH_DAMAGE_MULTIPLIER = 1.5f;
    [SyncVar]
    private bool tempest_on = false;

    public GameObject kam_wind_slash;
    private const float _skill1_cooldown = 12.0f;


    // Skill 2 (Butterfly Step) Space
    private const string BUTTERFLY_STEP_SPEED_BUFF_NAME = "KAM_BUTTERFLY_STEP";
    private const float BUTTERFLY_STEP_SPEED_MAGNITUDE = 1.8f;
    private const float BUTTERFLY_STEP_SPEED_DURATION = 1.0f;
    private const float _skill2_cooldown = 10.0f;

    public DashingTrail kam_butterfly_step_trail;

    public override void OnStartClient()
    {
        base.OnStartClient();
        ability_primary.SetCooldown(_primary_cooldown);
        ability_reload.SetCooldown(0);
        ability_skill1.SetCooldown(_skill1_cooldown);
        ability_skill1.name = "Tempest";
        ability_skill2.SetCooldown(_skill2_cooldown);
        ability_skill2.name = "Butterfly Step";
        StartCoroutine(SlashTypeCounter());
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonUp(0))
            mouse_up = true;
        if (isServer)
            if (SA_stunned)
                tempest_on = false;
    }

    public override void Passive()
    {
        return;
    }

    protected override void OnExecute()
    {
        base.OnExecute();
        ability_skill1.Reset();
        ability_skill2.Reset();
    }

    protected override void OnAssist()
    {
        base.OnAssist();
        ability_skill1.Reset();
        ability_skill2.Reset();
    }

    // ------------------------------------------------- Katana -------------------------------------------------
    public override void PrimaryAttack()
    {
        if (!mouse_up || tempest_on)
            return;
        mouse_up = false;
        KamSlashView ksv = Instantiate(kam_slash_view[slash_num]);
        ksv.transform.position = this.transform.position;
        ksv.transform.rotation = this.transform.rotation;
        CmdMakeKamSlash(slash_num);

        slash_timeout = 0.4f;
        slash_num++;
        if (slash_num == kam_slash_view.Length)
        {
            slash_num = 0;
            CmdInflictLockPrimary(0.5f);
        }
    }

    private IEnumerator SlashTypeCounter()
    {
        while (true)
        {
            if (slash_timeout > 0)
                slash_timeout -= Time.deltaTime;
            else
                slash_timeout = 0;
            if (slash_timeout == 0)
                slash_num = 0;
            yield return null;
        }
    }

    [Command]
    private void CmdMakeKamSlash(int num)
    {
        KamSlashLogic ksl = Instantiate(kam_slash_logic[num]);
        ksl.transform.position = this.transform.position;
        ksl.transform.rotation = this.transform.rotation;

        ksl.owner_id = netId;
        ksl.PreSpawnChangeTeam(GetTeam());
        NetworkServer.Spawn(ksl.gameObject);
        RpcMakeKamSlash(num);
    }

    [ClientRpc]
    private void RpcMakeKamSlash(int num)
    {
        if (player == Player.mine)
            return;
        KamSlashView ksv = Instantiate(kam_slash_view[num]);
        ksv.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        ksv.transform.position = this.transform.position;
        ksv.transform.rotation = this.transform.rotation;
    }


    public override void Reload()
    {
    }

    // ------------------------------------------------- Tempest -------------------------------------------------
    public override void Skill1()
    {
        StartCoroutine(Tempest());
    }

    private IEnumerator Tempest()
    {
        LocalAddMovespeedMultiplier(TEMPEST_MOVEMENT_MAGNITUDE, TEMPEST_DURATION, new AbilityInfo(TEMPEST_NAME, this.netId), this.netId);
        tempest_on = true;
        CmdChangeTempestMode(true);
        yield return new WaitForSeconds(TEMPEST_DURATION);
        CmdChangeTempestMode(false);
    }

    [Command]
    private void CmdChangeTempestMode(bool change_to)
    {
        tempest_on = change_to;
    }

    public override void ChangeHealth(Character source, float amount)
    {
        if (amount >= 0)
            base.ChangeHealth(source, amount);
        else
        {
            if (source == GetClosestEnemy())
                amount *= PASSIVE_MULTIPLIER;
            if (tempest_on)
            {
                if (source.GetType() == typeof(Squeek))
                {
                    base.ChangeHealth(source, amount);
                }
                else
                {
                    float AngleRad = Mathf.Atan2(source.transform.position.y - transform.position.y, source.transform.position.x - transform.position.x);
                    float AngleDeg = (180 / Mathf.PI) * AngleRad;
                    Quaternion rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
                    CmdMakeWindSlash(rotation, amount * TEMPEST_WIND_SLASH_DAMAGE_MULTIPLIER);
                }
            }
            else
                base.ChangeHealth(source, amount);
        }
    }

    [Command]
    private void CmdMakeWindSlash(Quaternion rotation, float damage)
    {
        GameObject kws = Instantiate<GameObject>(kam_wind_slash);
        kws.transform.position = this.transform.position;
        kws.transform.rotation = rotation;
        kws.GetComponent<KamWindSlashLogic>().owner = this;
        kws.GetComponent<KamWindSlashLogic>().damage = Mathf.Abs(damage);
        kws.GetComponent<KamWindSlashLogic>().PreSpawnChangeTeam(this.GetTeam());
        NetworkServer.Spawn(kws);
    }


    // ------------------------------------------------- Butterfly Step -------------------------------------------------
    public override void Skill2()
    {
        if (SA_rooted)
        {
            ability_skill2.Reset();
            return;
        }
        StartCoroutine(ButterflyStep());
    }


    private IEnumerator ButterflyStep()
    {
        // show trail
        //DashingTrail dt = Instantiate(kam_butterfly_step_trail);
        //dt.owner = this;
        //CmdDashTrail();

        yield return null;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CmdAskServerMoveCharacter(mouse);
        transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // Give movespeed buff
        LocalAddMovespeedMultiplier(BUTTERFLY_STEP_SPEED_MAGNITUDE, BUTTERFLY_STEP_SPEED_DURATION, new AbilityInfo(BUTTERFLY_STEP_SPEED_BUFF_NAME, this.netId), this.netId);
        yield return null;
    }

    [Command]
    private void CmdAskServerMoveCharacter(Vector2 pos)
    {
        RpcMoveCharacter(pos);
    }

    // Move this character on other people's screens.
    [ClientRpc]
    private void RpcMoveCharacter(Vector2 pos)
    {
        if (player == Player.mine)
            return;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }


    [Command]
    private void CmdDashTrail()
    {
        RpcDashTrail();
    }

    [ClientRpc]
    private void RpcDashTrail()
    {
        if (player == Player.mine)
            return;
        DashingTrail dt = Instantiate(kam_butterfly_step_trail);
        dt.owner = this;
    }


    // ------------------------------------------------- GUI -------------------------------------------------
    protected override void OnGUI()
    {
        base.OnGUI();
        if (Player.mine.character != this)
            return;
        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 80, 300, 100), "Your Health: " + (int)GetHealth() + " / " + (int)max_health);

        //GUI.Label(new Rect(30, Screen.height - 100, 300, 100), primary.ToString());
        
        GUI.Label(new Rect(30, Screen.height - 80, 300, 100), ability_skill1.ToString());
        GUI.Label(new Rect(30, Screen.height - 60, 300, 100), ability_skill2.ToString());
    }


}