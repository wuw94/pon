using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class Squeak : Character
{

	// Characteristics
	public override float max_health { get { return 200; } set { throw new NotImplementedException(); } }
	public override float max_speed { get { return 3.5f; } set { throw new NotImplementedException(); } }

	// Passive
	private const float PASSIVE_WAIT_TIME_BEFORE_REGEN = 5.0f;
	private const float PASSIVE_REGEN_RATE = 10.0f; // this is per second

	// Primary Weapon
	private const float _primary_cooldown = 0;
	private const float PRIMARY_DAMAGE = 50.0f;

	[SyncVar(hook = "OnUpdateLatch")]
	private NetworkInstanceId latched_to_id;
	public Character latched_to;

	public GameObject[] latch_beam_particles = new GameObject[10];
	public Vector2 mid_point;
	public Vector2 end_point;

	// Skill 1 (Piggyback)
	private const float _skill1_cooldown = 1.0f;

	// Skill 2 (Transience)
	private const float _skill2_cooldown = 1.0f;

	public override void OnStartClient()
	{
		base.OnStartClient();
		ability_primary.SetCooldown(_primary_cooldown);
		ability_reload.SetCooldown(0);
		ability_skill1.SetCooldown(_skill1_cooldown);
		ability_skill1.name = "Piggyback";
		ability_skill2.SetCooldown(_skill2_cooldown);
		ability_skill2.name = "Transience";

		for (int i = 0; i < 10; i++)
		{
			latch_beam_particles[i] = Instantiate(latch_beam_particles[i]);
		}
	}

	public override void Update()
	{
		if (hasAuthority)
		{
			if (Input.GetMouseButtonUp(0))
				CmdChangeLatch(NetworkInstanceId.Invalid);
		}

		for (int i = 0; i < 10; i++)
		{
			latch_beam_particles[i].transform.position = Bezier(this.transform.position, 
		}
	}

	public override void Passive()
	{
		// Manage regen
		if (Time.time - time_of_recent_damage > PASSIVE_WAIT_TIME_BEFORE_REGEN)
			ChangeHealth(this.player, Time.deltaTime * PASSIVE_REGEN_RATE);

	}

	// ------------------------------------------------- Beam -------------------------------------------------
	public override void PrimaryAttack()
	{
		if (this.latched_to == null)
			CmdChangeLatch(GetClosestCharacterToMouse().netId);
		LocalAffectLatched();
		CmdAffectLatched();
	}

	private void LocalAffectLatched()
	{
		if (latched_to.GetTeam() == this.GetTeam())
			latched_to.ChangeHealth(this.player, Time.deltaTime * PRIMARY_DAMAGE);
		else
			latched_to.ChangeHealth(this.player, -Time.deltaTime * PRIMARY_DAMAGE);
	}

	[Command]
	private void CmdAffectLatched()
	{
		if (latched_to.GetTeam() == this.GetTeam())
			latched_to.ChangeHealth(this.player, Time.deltaTime * PRIMARY_DAMAGE);
		else
			latched_to.ChangeHealth(this.player, -Time.deltaTime * PRIMARY_DAMAGE);
	}

	[Command]
	private void CmdChangeLatch(NetworkInstanceId id)
	{
		this.latched_to_id = id;
	}

	private void OnUpdateLatch(NetworkInstanceId id)
	{
		this.latched_to_id = id;
		if (id == NetworkInstanceId.Invalid)
			this.latched_to = null;
		else
			this.latched_to = ClientScene.FindLocalObject(this.latched_to_id).GetComponent<Character>();
	}


	private Vector2 Bezier(Vector2 start, Vector2 middle, Vector2 end, float t)
	{
		return new Vector2(Mathf.Pow(1-t,2)*start.x+(1-t)*2*t*middle.x+t*t*end.x,Mathf.Pow(1-t,2)*start.y+(1-t)*2*t*middle.y+t*t*end.y);
	}



	// Beam no reload
	public override void Reload()
	{
	}

	// ------------------------------------------------- Piggyback -------------------------------------------------
	// To execute an ability we
	// create local
	// command to create logic
	// rpc to create image for others
	// to send a command we need authority. authority is only gained through spawn
	public override void Skill1()
	{
	}

	// ------------------------------------------------- Transience -------------------------------------------------
	public override void Skill2()
	{
	}


	// ------------------------------------------------- GUI -------------------------------------------------
	protected override void OnGUI()
	{
		base.OnGUI();
		if (Player.mine.character != this)
			return;
		GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 80, 300, 100), "Your Health: " + (int)GetHealth() + " / " + (int)max_health);

		GUI.Label(new Rect(30, Screen.height - 100, 300, 100), "Beam");

		GUI.Label(new Rect(30, Screen.height - 80, 300, 100), ability_skill1.ToString());
		GUI.Label(new Rect(30, Screen.height - 60, 300, 100), ability_skill2.ToString());
	}


}
