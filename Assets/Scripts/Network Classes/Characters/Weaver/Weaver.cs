using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class Weaver : Character
{
    // Characteristics
    public override float max_health { get { return 200; } set { throw new NotImplementedException(); } }
    public override float max_speed { get { return 3.5f; } set { throw new NotImplementedException(); } }

    // Primary Weapon
    [SerializeField]
    private Firearm primary;
    private const float _primary_cooldown = 0.3f;

    // Skill 1 (Piercing Thread) LShift
    private const float _skill1_cooldown = 12.0f;

    public WeaverPiercingThread weaver_piercing_thread;

    // Skill 2 (Tumble) Space
    private const float _skill2_cooldown = 8.0f;
    private bool _skill2_can_echo = false;

	public DashingTrail weaver_tumble_trail;

    public override void OnStartClient()
    {
        base.OnStartClient();
        ability_primary.SetCooldown(_primary_cooldown);
        ability_reload.SetCooldown(0);
        ability_skill1.SetCooldown(_skill1_cooldown);
        ability_skill1.name = "Piercing Thread";
        ability_skill2.SetCooldown(_skill2_cooldown);
        ability_skill2.name = "Tumble";
    }

    public override void Passive()
    {
        return;
    }

    // ------------------------------------------------- Pistol -------------------------------------------------
    public override void PrimaryAttack()
    {
        if (!primary.is_reloading)
        {
            ShakeCamera(0.04f, 0.07f, Quaternion.Euler(0, 0, GetMouseDirection(attacking_offset.position)));
            primary.Fire(GetMouseDirection(attacking_offset.position));
        }
        else
            ability_primary.Reset();
    }

    public override void Reload()
    {
        StartCoroutine(primary.Reload());
    }

    // ------------------------------------------------- Piercing Thread -------------------------------------------------
    public override void Skill1()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        weaver_piercing_thread.ShowLocal(attacking_offset.position, this.transform.rotation);
        CmdMakePiercingThread();
        StartCoroutine(PiercingThread());
    }
    
    [Command]
    private void CmdMakePiercingThread()
    {
        WeaverPiercingThread wpt = Instantiate<WeaverPiercingThread>(weaver_piercing_thread);
        NetworkServer.SpawnWithClientAuthority(wpt.gameObject, this.player.connectionToClient);
        wpt.Make(attacking_offset.position, this.transform.rotation, this.GetTeam(), this.player.connectionToClient);
    }
    
    private IEnumerator PiercingThread()
    {
        CmdInflictStun(0.5f);
        yield return new WaitForSeconds(0.5f);
    }


    // ------------------------------------------------- Tumble -------------------------------------------------
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
        {
            ability_skill2.Reset();
            return;
        }
        
        if (!_skill2_can_echo)
            StartCoroutine(Tumble());
        else
            StartCoroutine(TumbleEcho());
    }


    private IEnumerator Tumble()
    {
		// show trail
		DashingTrail dt = Instantiate(weaver_tumble_trail);
		dt.owner = this;
		CmdDashTrail();

        for (int i = 0; i < 5; i++)
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
            dir = Vector2.ClampMagnitude(dir * 18, 18);
            GetComponent<Rigidbody2D>().velocity = dir;
            yield return new WaitForSeconds(0.02f);
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(TumbleEchoCooldown());
    }

    private IEnumerator TumbleEchoCooldown()
    {
        ability_skill2.Reset();
        _skill2_can_echo = true;
        yield return new WaitForSeconds(1);
        _skill2_can_echo = false;
        ability_skill2.Use();
    }

    private IEnumerator TumbleEcho()
    {
		DashingTrail dt = Instantiate(weaver_tumble_trail);
		dt.owner = this;
		CmdDashTrail();

        _skill2_can_echo = false;
        for (int i = 0; i < 5; i++)
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

            dir = Vector2.ClampMagnitude(dir * 18, 18);
            GetComponent<Rigidbody2D>().velocity = dir;
            yield return new WaitForSeconds(0.02f);
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ability_skill2.Use();
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
		DashingTrail dt = Instantiate(weaver_tumble_trail);
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
            for (int i = 0; i < 100 - primary.reload_percent; i += 5)
            {
                s += "|";
            }
            GUI.Label(new Rect(Screen.width / 2 + 30, Screen.height / 2 - 30, 300, 100), s);
        }
        GUI.Label(new Rect(30, Screen.height - 80, 300, 100), ability_skill1.ToString());
        GUI.Label(new Rect(30, Screen.height - 60, 300, 100), ability_skill2.ToString());
    }


}