using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Rockjaw : Character
{
    // Characteristics
    public override float max_health { get { return 250; } set { throw new NotImplementedException(); } }
    public override float max_speed { get { return 3.5f; } set { throw new NotImplementedException(); } }

    // Passive
    private string BATTLE_HUNGER_NAME = "ROCKJAW_BATTLE_HUNGER";
    private const float BATTLE_HUNGER_DAMAGE_REDUCTION_MIN = 1.0f;
    private const float BATTLE_HUNGER_DAMAGE_REDUCTION_MAX = 0.5f;
    private const float BATTLE_HUNGER_SPEED_MIN = 1.0f;
    private const float BATTLE_HUNGER_SPEED_MAX = 1.2f;
    [SyncVar]
    private float BATTLE_HUNGER_MAGNITUDE = 0;
    private const float BATTLE_HUNGER_TIMEOUT = 4;
    private float BATTLE_HUNGER_TIMER = 0;

    // Primary Weapon
    [SerializeField]
    private Shotgun primary;
    private const float _primary_cooldown = 0.4f;

    // Skill 1 (Impale)
    public RockjawCrunchView rockjaw_crunch_view;
    public RockjawCrunchLogic rockjaw_crunch_logic;
    private const float _skill1_cooldown = 5.0f;

    // Skill 2 (Blitz)
    private const float _skill2_cooldown = 8.0f;
	public DashingTrail rockjaw_blitz;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(BattleHunger());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ability_primary.SetCooldown(_primary_cooldown);
        ability_reload.SetCooldown(0);
        ability_skill1.SetCooldown(_skill1_cooldown);
        ability_skill1.name = "Crunch";
        ability_skill2.SetCooldown(_skill2_cooldown);
        ability_skill2.name = "Blitz";
    }

    public override void Passive()
    {
        return;
    }

    protected override void OnDamagedOther(Character other, float amount)
    {
        base.OnDamagedOther(other, amount);
        BATTLE_HUNGER_TIMER = BATTLE_HUNGER_TIMEOUT;
    }

    protected override void OnDamagedByOther(Character other, float amount)
    {
        base.OnDamagedByOther(other, amount);
        BATTLE_HUNGER_TIMER = BATTLE_HUNGER_TIMEOUT;
    }

    private IEnumerator BattleHunger()
    {
        while (true)
        {
            BATTLE_HUNGER_TIMER = Mathf.Clamp(BATTLE_HUNGER_TIMER - Time.deltaTime, 0, BATTLE_HUNGER_TIMEOUT);
            if (BATTLE_HUNGER_TIMER == 0)
                BATTLE_HUNGER_MAGNITUDE -= Time.deltaTime / 4;
            else
                BATTLE_HUNGER_MAGNITUDE += Time.deltaTime / 4;
            BATTLE_HUNGER_MAGNITUDE = Mathf.Clamp(BATTLE_HUNGER_MAGNITUDE, 0, 1);
            CmdAddMovespeedMultiplier(Mathf.Lerp(BATTLE_HUNGER_SPEED_MIN, BATTLE_HUNGER_SPEED_MAX, BATTLE_HUNGER_MAGNITUDE), 0.2f, new AbilityInfo(BATTLE_HUNGER_NAME, this.netId), this.netId);
            yield return new WaitForEndOfFrame();
        }
    }

    public override void ChangeHealth(Character source, float amount)
    {
        if (amount < 0)
            base.ChangeHealth(source, amount * Mathf.Lerp(BATTLE_HUNGER_DAMAGE_REDUCTION_MIN, BATTLE_HUNGER_DAMAGE_REDUCTION_MAX, BATTLE_HUNGER_MAGNITUDE));
        else
            base.ChangeHealth(source, amount);
    }

    // ------------------------------------------------- Shotgun -------------------------------------------------
    public override void PrimaryAttack()
    {
        if (!primary.is_reloading)
        {
            ShakeCamera(0.05f, 0.09f, Quaternion.Euler(0, 0, GetMouseDirection(attacking_offset.position)));
            primary.Fire(GetMouseDirection(attacking_offset.position));
        }
        else
            ability_primary.Reset();
    }

    public override void Reload()
    {
        StartCoroutine(primary.Reload());
    }

    // ------------------------------------------------- Impale -------------------------------------------------
    // To execute an ability we
    // create local
    // command to create logic
    // rpc to create image for others
    // to send a command we need authority. authority is only gained through spawn
    public override void Skill1()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        RockjawCrunchView rcv = Instantiate(rockjaw_crunch_view);
        rcv.transform.position = this.attacking_offset.position + transform.rotation * (Vector2.up * 0.5f);
        rcv.transform.rotation = Quaternion.identity;
        CmdMakeRockjawCrunch();
        StartCoroutine(RockjawCrunch());
    }

    [Command]
    private void CmdMakeRockjawCrunch()
    {
        RockjawCrunchLogic l = Instantiate<RockjawCrunchLogic>(rockjaw_crunch_logic);
        l.transform.position = this.attacking_offset.position + transform.rotation * (Vector2.up * 0.5f);
        l.owner_id = netId;
        l.PreSpawnChangeTeam(GetTeam());
        NetworkServer.Spawn(l.gameObject);
        RpcMakeRockjawCrunch();
    }

    [ClientRpc]
    private void RpcMakeRockjawCrunch()
    {
        if (player == Player.mine)
            return;
        RockjawCrunchView rcv = Instantiate(rockjaw_crunch_view);
        rcv.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        rcv.transform.position = this.attacking_offset.position + transform.rotation * (Vector2.up * 0.5f);
        rcv.transform.rotation = Quaternion.identity;
    }

    private IEnumerator RockjawCrunch()
    {
        CmdInflictStun(0.1f);
        yield return new WaitForSeconds(0.1f);
    }


    // ------------------------------------------------- Blitz -------------------------------------------------
    public override void Skill2()
    {
        if (SA_rooted)
        {
            ability_skill2.Reset();
            return;
        }

        CmdInflictRoot(0.1f);

        Vector3 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            dir += Camera.main.transform.rotation * Vector2.up;
        if (Input.GetKey(KeyCode.A))
            dir += Camera.main.transform.rotation * Vector2.left;
        if (Input.GetKey(KeyCode.S))
            dir += Camera.main.transform.rotation * Vector2.down;
        if (Input.GetKey(KeyCode.D))
            dir += Camera.main.transform.rotation * Vector2.right;

        if (dir == Vector3.zero)
            ability_skill2.Reset();
        else
            StartCoroutine(Dash());
    }

    

    private IEnumerator Dash()
    {
		DashingTrail dt = Instantiate(rockjaw_blitz);
		dt.owner = this;
		CmdDashTrail();

        StopCoroutine(primary.Reload());
        primary.reload_percent = 100;
        primary.ammunition.Refill();
        for (int i = 0; i < 4; i++)
        {

            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
                dir += Vector2.up;
            if (Input.GetKey(KeyCode.A))
                dir += Vector2.left;
            if (Input.GetKey(KeyCode.S))
                dir += Vector2.down;
            if (Input.GetKey(KeyCode.D))
                dir += Vector2.right;

            dir = Vector2.ClampMagnitude(dir * 30, 30);
            GetComponent<Rigidbody2D>().velocity = dir;
            yield return new WaitForSeconds(0.02f);
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
		DashingTrail dt = Instantiate(rockjaw_blitz);
		dt.owner = this;
	}


    // ------------------------------------------------- GUI -------------------------------------------------
    protected override void OnGUI()
    {
        base.OnGUI();
        if (Player.mine.character != this)
            return;
        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 80, 300, 100), "Your Health: " + (int)GetHealth() + " / " + (int)max_health);
        
        GUI.Label(new Rect(30, Screen.height - 100, 300, 100), primary.ToString());
        if (primary.is_reloading)
        {
            string s = "";
            for (int i = 0; i < 100 - primary.reload_percent; i+= 5)
            {
                s += "|";
            }
            GUI.Label(new Rect(Screen.width / 2 + 30, Screen.height / 2 - 30, 300, 100), s);
        }
        GUI.Label(new Rect(30, Screen.height - 80, 300, 100), ability_skill1.ToString());
        GUI.Label(new Rect(30, Screen.height - 60, 300, 100), ability_skill2.ToString());
    }

    
}
