using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class Squeek : Character
{
	// Characteristics
	public override float max_health { get { return 200; } set { throw new NotImplementedException(); } }
	public override float max_speed { get { return 3.5f; } set { throw new NotImplementedException(); } }

	// Passive
	private const float PASSIVE_WAIT_TIME_BEFORE_REGEN = 3.0f;
	private const float PASSIVE_REGEN_RATE = 20.0f; // this is per second

	// Primary Weapon (Photon Beam)
	private const float _primary_cooldown = 0;
    private const float PRIMARY_HEAL = 60.0f;
	private const float PRIMARY_DAMAGE = 40.0f;

	[SyncVar(hook = "OnUpdateLatch")]
	private NetworkInstanceId latched_to_id;
	public Character latched_to;

    private const int PHOTON_BEAM_PARTICLE_COUNT = 40;
    private const float PHOTON_BEAM_RANGE = 3.0f;
    private const float PHOTON_BEAM_TOTAL_STRENGTH = 1.0f;
    [SyncVar(hook = "OnUpdatePhotonBeamCurrentStrength")]
    private float photon_beam_current_strength = PHOTON_BEAM_TOTAL_STRENGTH;
    public GameObject photon_beam_particle;
	public GameObject[] latch_beam_particles;
    public float[] latch_beam_particles_width;

	// Skill 1 (Piggyback)
	private const float _skill1_cooldown = 0.5f;
    private const string PIGGYBACK_MOVESPEED_SOURCE_NAME = "SQUEEK_PIGGYBACK";
    private const float PIGGYBACK_MOVESPEED_MULTIPLIER = 0.8f;
    private const float PIGGYBACK_MOVESPEED_MULTIPLIER_TICK_DURATION = 0.2f;
    private const float PIGGYBACK_RANGE = 1.0f;
    [SyncVar(hook = "OnUpdateMount")]
    public bool mounted;
    private GameObject squeek_mount_on;
    public Sprite squeek_mount_on_l;
    public Sprite squeek_mount_on_r;

    // Skill 2 (Transience)
    private const float _skill2_cooldown = 12.0f;
    private const string TRANSIENCE_MOVESPEED_SOURCE_NAME = "SQUEEK_TRANSIENCE";
    private const float TRANSIENCE_MULTIPLIER_ALLY_MAX = 1.7f;
    private const float TRANSIENCE_MULTIPLIER_ALLY_MIN = 1.4f;
    private const float TRANSIENCE_MULTIPLIER_ENEMY_MAX = 0.5f;
    private const float TRANSIENCE_MULTIPLIER_ENEMY_MIN = 0.6f;
    private const float TRANSIENCE_DURATION = 3.0f;
    private const float TRANSIENCE_TICK_DURATION = 0.2f;

	public override void OnStartClient()
	{
		base.OnStartClient();
		ability_primary.SetCooldown(_primary_cooldown);
		ability_reload.SetCooldown(0);
		ability_skill1.SetCooldown(_skill1_cooldown);
		ability_skill1.name = "Piggyback";
		ability_skill2.SetCooldown(_skill2_cooldown);
		ability_skill2.name = "Transience";

        latch_beam_particles = new GameObject[PHOTON_BEAM_PARTICLE_COUNT];
        latch_beam_particles_width = new float[PHOTON_BEAM_PARTICLE_COUNT];
        for (int i = 0; i < PHOTON_BEAM_PARTICLE_COUNT; i++)
        {
            latch_beam_particles[i] = Instantiate(photon_beam_particle);
            latch_beam_particles_width[i] = latch_beam_particles[i].transform.localScale.x + 0.06f * i;
            latch_beam_particles[i].transform.localScale = new Vector3(latch_beam_particles_width[i], 0, latch_beam_particles[i].transform.localScale.z);
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        squeek_mount_on = Instantiate(Resources.Load<GameObject>("Characters/Squeek/SqueekMountOn"));
        StartCoroutine(PhotonBeamOutOfRangeTimer());
    }

    public override void Update()
    {
        base.Update();
        PrimaryUpdate();
        Skill1Update();
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
        {
            Character c = GetClosestCharacterToMouse();
            if (c != null && Vector2.Distance(this.transform.position, c.transform.position) < PHOTON_BEAM_RANGE)
            {
                LocalChangeLatch(c.netId);
                CmdChangeLatch(c.netId);
            }
        }
	}

	private void LocalAffectLatched(float strength)
	{
        if (isServer)
            return;
        if (latched_to == null)
            return;
        if (latched_to.GetTeam() == this.GetTeam())
            latched_to.ChangeHealth(this.player, Time.deltaTime * Mathf.Lerp(0, PRIMARY_HEAL, strength));
        else
            latched_to.ChangeHealth(this.player, -Time.deltaTime * Mathf.Lerp(0, PRIMARY_DAMAGE, strength));
	}

    [Command]
    private void CmdAffectLatched(float strength)
	{
        photon_beam_current_strength = strength;
        if (latched_to == null)
            return;
        if (latched_to.GetTeam() == this.GetTeam())
            latched_to.ChangeHealth(this.player, Time.deltaTime * Mathf.Lerp(0, PRIMARY_HEAL, strength / PHOTON_BEAM_TOTAL_STRENGTH));
        else
            latched_to.ChangeHealth(this.player, -Time.deltaTime * Mathf.Lerp(0, PRIMARY_DAMAGE, strength / PHOTON_BEAM_TOTAL_STRENGTH));
    }

    private void LocalChangeLatch(NetworkInstanceId id)
    {
        this.latched_to_id = id;
        if (id == NetworkInstanceId.Invalid)
            this.latched_to = null;
        else
            this.latched_to = ClientScene.FindLocalObject(id).GetComponent<Character>();
    }

	[Command]
	private void CmdChangeLatch(NetworkInstanceId id)
	{
		this.latched_to_id = id;
	}

	private void OnUpdateLatch(NetworkInstanceId id)
	{
        if (hasAuthority)
            return;
		this.latched_to_id = id;
		if (id == NetworkInstanceId.Invalid)
			this.latched_to = null;
		else
			this.latched_to = ClientScene.FindLocalObject(this.latched_to_id).GetComponent<Character>();
	}


    private void PrimaryUpdate()
    {
        if (hasAuthority)
        {
            if (mounted || Input.GetMouseButton(0))
            {
                if (latched_to != null)
                {
                    if (Vector2.Distance(this.transform.position, latched_to.transform.position) < PHOTON_BEAM_RANGE && latched_to.is_visible)
                    {
                        photon_beam_current_strength += 1.5f * Time.deltaTime;
                    }
                    LocalAffectLatched(photon_beam_current_strength);
                    CmdAffectLatched(photon_beam_current_strength);
                }
            }
            if (!mounted)
            {
                if (this.IsDead() || !Input.GetMouseButton(0) || (latched_to != null && latched_to.IsDead()))
                {
                    LocalChangeLatch(NetworkInstanceId.Invalid);
                    CmdChangeLatch(NetworkInstanceId.Invalid);
                }
            }
        }

        if (latched_to == null)
            for (int i = 0; i < PHOTON_BEAM_PARTICLE_COUNT; i++)
            {
                latch_beam_particles[i].GetComponent<SpriteRenderer>().color = new Color(this.GetComponent<SpriteRenderer>().color.r, this.GetComponent<SpriteRenderer>().color.g, this.GetComponent<SpriteRenderer>().color.b, 0);
                latch_beam_particles[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            }
        else
        {
            for (int i = 0; i < PHOTON_BEAM_PARTICLE_COUNT; i++)
            {
                latch_beam_particles[i].GetComponent<SpriteRenderer>().color = new Color(this.GetComponent<SpriteRenderer>().color.r, this.GetComponent<SpriteRenderer>().color.g, this.GetComponent<SpriteRenderer>().color.b, 0.1f);
                latch_beam_particles[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.04f);
                if (mounted)
                    latch_beam_particles[i].transform.position = Vector2.Lerp(this.attacking_offset.position, latched_to.transform.position, i * 1.0f / PHOTON_BEAM_PARTICLE_COUNT);
                else
                    latch_beam_particles[i].transform.position = Bezier(this.attacking_offset.position, this.attacking_offset.position + this.transform.rotation * (Vector2.up * 1.0f), latched_to.transform.position, i * 1.0f / PHOTON_BEAM_PARTICLE_COUNT);
            }
            for (int i = 0; i < PHOTON_BEAM_PARTICLE_COUNT - 1; i++)
            {
                float AngleRad = Mathf.Atan2(latch_beam_particles[i + 1].transform.position.y - latch_beam_particles[i].transform.position.y, latch_beam_particles[i + 1].transform.position.x - latch_beam_particles[i].transform.position.x);
                latch_beam_particles[i].transform.rotation = Quaternion.Euler(0, 0, (180 / Mathf.PI) * AngleRad - 90);
                latch_beam_particles[i].transform.localScale = new Vector3(Mathf.Lerp(0, latch_beam_particles_width[i], photon_beam_current_strength / PHOTON_BEAM_TOTAL_STRENGTH), Vector2.Distance(latch_beam_particles[i].transform.position, latch_beam_particles[i + 1].transform.position) * 15, latch_beam_particles[i].transform.localScale.z);
            }
            float angle = Mathf.Atan2(latched_to.transform.position.y - latch_beam_particles[PHOTON_BEAM_PARTICLE_COUNT - 1].transform.position.y, latched_to.transform.position.x - latch_beam_particles[PHOTON_BEAM_PARTICLE_COUNT - 1].transform.position.x);
            latch_beam_particles[PHOTON_BEAM_PARTICLE_COUNT - 1].transform.rotation = Quaternion.Euler(0, 0, (180 / Mathf.PI) * angle - 90);
        }
    }

    private IEnumerator PhotonBeamOutOfRangeTimer()
    {
        while (true)
        {
            if (this.player == Player.mine)
            {
                photon_beam_current_strength = Mathf.Clamp(photon_beam_current_strength - Time.deltaTime, 0, PHOTON_BEAM_TOTAL_STRENGTH);
                if (photon_beam_current_strength == 0)
                {
                    LocalChangeLatch(NetworkInstanceId.Invalid);
                    CmdChangeLatch(NetworkInstanceId.Invalid);
                }
                CmdSetPhotonBeamCurrentStrength(photon_beam_current_strength);
            }
            yield return null;
        }
    }

    private void CmdSetPhotonBeamCurrentStrength(float value)
    {
        photon_beam_current_strength = value;
    }

    private void OnUpdatePhotonBeamCurrentStrength(float new_value)
    {
        if (this.player != Player.mine)
            photon_beam_current_strength = new_value;
    }

    private Vector2 Bezier(Vector2 start, Vector2 middle, Vector2 end, float t)
    {
        return new Vector2(Mathf.Pow(1 - t, 2) * start.x + (1 - t) * 2 * t * middle.x + t * t * end.x, Mathf.Pow(1 - t, 2) * start.y + (1 - t) * 2 * t * middle.y + t * t * end.y);
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
        if (mounted)
            StartCoroutine(Unmount());
        else
        {
            bool can_mount = false;
            // check if photon beam is active and on who
            Character c = null;
            if (latched_to != null)
                c = latched_to;
            else
                c = GetClosestAlly(); 

            if (c != null && c.GetType() != typeof(Squeek) && c.GetTeam() == this.GetTeam() && Vector2.Distance(this.transform.position, c.transform.position) < PIGGYBACK_RANGE)
            {
                can_mount = true;
                foreach (Squeek other in FindObjectsOfType<Squeek>())
                    if (other.mounted && other.latched_to == this.latched_to)
                        can_mount = false;
            }
            if (can_mount)
            {
                LocalChangeLatch(c.netId);
                CmdChangeLatch(c.netId);
                StartCoroutine(Mount());
            }
            else
                ability_skill1.Reset();
        }
	}

    private IEnumerator Mount()
    {
        LocalChangeMounted(true);
        CmdChangeMounted(true);
        yield return new WaitForSeconds(0.1f);
        if (Camera.main != null && Camera.main.GetComponent<LerpFollow>() != null)
            Camera.main.GetComponent<LerpFollow>().target = latched_to.transform;
        vision.transform.localPosition = new Vector3(0, 0.2f, 0);
        look_for_characters.transform.localPosition = new Vector3(0, 0.2f, 0);
    }

    private IEnumerator Unmount()
    {
        CmdInflictStun(0.3f);
        LocalChangeMounted(false);
        CmdChangeMounted(false);
        Vector2 dir = this.transform.rotation * (Vector2.down * 2.0f);
        dir = Vector2.ClampMagnitude(dir * 2, 2);
        for (int i = 0; i < 5; i++)
        {
            GetComponent<Rigidbody2D>().velocity = dir;
            yield return new WaitForSeconds(0.02f);
        }
        if (Camera.main != null && Camera.main.GetComponent<LerpFollow>() != null)
            Camera.main.GetComponent<LerpFollow>().target = this.transform;
        if (vision != null)
            vision.transform.localPosition = Vector3.zero;
        if (look_for_characters != null)
            look_for_characters.transform.localPosition = Vector3.zero;
    }

    private void LocalChangeMounted(bool change_to)
    {
        mounted = change_to;
        GetComponent<CircleCollider2D>().isTrigger = mounted;
    }

    [Command]
    private void CmdChangeMounted(bool change_to)
    {
        mounted = change_to;
    }

    private void OnUpdateMount(bool change_to)
    {
        mounted = change_to;
        GetComponent<CircleCollider2D>().isTrigger = mounted;
    }
    
    private void Skill1Update()
    {
        if (hasAuthority)
        {
            if (mounted && (IsDead() || latched_to == null || latched_to.IsDead()))
                StartCoroutine(Unmount());
            if (mounted)
            {
                if (latched_to == null)
                    StartCoroutine(Unmount());
                squeek_mount_on.transform.position = latched_to.transform.position;
                squeek_mount_on.GetComponent<SpriteRenderer>().color = new Color(squeek_mount_on.GetComponent<SpriteRenderer>().color.r, squeek_mount_on.GetComponent<SpriteRenderer>().color.g, squeek_mount_on.GetComponent<SpriteRenderer>().color.b, Mathf.Lerp(squeek_mount_on.GetComponent<SpriteRenderer>().color.a, 0, 50 * Time.deltaTime));
                CmdAddMovespeedMultiplier(PIGGYBACK_MOVESPEED_MULTIPLIER, PIGGYBACK_MOVESPEED_MULTIPLIER_TICK_DURATION, PIGGYBACK_MOVESPEED_SOURCE_NAME, latched_to_id);
            }
            if (!mounted)
            {
                Character c = null;
                if (latched_to != null)
                    c = latched_to;
                else
                    c = GetClosestAlly();
                if (c == null || c.GetTeam() != this.GetTeam() || Vector2.Distance(this.transform.position, c.transform.position) > PIGGYBACK_RANGE)
                    squeek_mount_on.GetComponent<SpriteRenderer>().color = new Color(squeek_mount_on.GetComponent<SpriteRenderer>().color.r, squeek_mount_on.GetComponent<SpriteRenderer>().color.g, squeek_mount_on.GetComponent<SpriteRenderer>().color.b, 0);
                else
                {
                    squeek_mount_on.transform.position = c.transform.position;
                    squeek_mount_on.GetComponent<SpriteRenderer>().sprite = (this.transform.position.x < c.transform.position.x) ? squeek_mount_on_r : squeek_mount_on_l;
                    squeek_mount_on.GetComponent<SpriteRenderer>().color = new Color(squeek_mount_on.GetComponent<SpriteRenderer>().color.r, squeek_mount_on.GetComponent<SpriteRenderer>().color.g, squeek_mount_on.GetComponent<SpriteRenderer>().color.b, Mathf.Lerp(squeek_mount_on.GetComponent<SpriteRenderer>().color.a, 1, 50 * Time.deltaTime));
                }
            }
        }
        if (mounted && latched_to != null)
        {
            this.transform.position = latched_to.transform.position + latched_to.transform.rotation * (Vector2.down * 0.2f);
            this.transform.rotation = latched_to.transform.rotation;
        }
    }

	// ------------------------------------------------- Transience -------------------------------------------------
	public override void Skill2()
	{
        StartCoroutine(Transience());
	}

    private IEnumerator Transience()
    {
        for (int i = 0; i < TRANSIENCE_DURATION / TRANSIENCE_TICK_DURATION; i++)
        {
            LocalAddMovespeedMultiplier(Mathf.Lerp(TRANSIENCE_MULTIPLIER_ALLY_MAX, TRANSIENCE_MULTIPLIER_ALLY_MIN, i * 1.0f / TRANSIENCE_DURATION / TRANSIENCE_TICK_DURATION), TRANSIENCE_TICK_DURATION * 2, TRANSIENCE_MOVESPEED_SOURCE_NAME, this.netId);
            if (latched_to != null)
            {
                if (latched_to.GetTeam() == this.GetTeam())
                    CmdAddMovespeedMultiplier(Mathf.Lerp(TRANSIENCE_MULTIPLIER_ALLY_MAX, TRANSIENCE_MULTIPLIER_ALLY_MIN, i * 1.0f / TRANSIENCE_DURATION / TRANSIENCE_TICK_DURATION), TRANSIENCE_TICK_DURATION * 2, TRANSIENCE_MOVESPEED_SOURCE_NAME, latched_to_id);
                else
                    CmdAddMovespeedMultiplier(Mathf.Lerp(TRANSIENCE_MULTIPLIER_ENEMY_MAX, TRANSIENCE_MULTIPLIER_ENEMY_MIN, i * 1.0f / TRANSIENCE_DURATION / TRANSIENCE_TICK_DURATION), TRANSIENCE_TICK_DURATION * 2, TRANSIENCE_MOVESPEED_SOURCE_NAME, latched_to_id);
            }
            yield return new WaitForSeconds(TRANSIENCE_TICK_DURATION);
        }
    }


	// ------------------------------------------------- GUI -------------------------------------------------
	protected override void OnGUI()
	{
		base.OnGUI();
		if (Player.mine.character != this)
			return;
		GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 80, 300, 100), "Your Health: " + (int)GetHealth() + " / " + (int)max_health);

		GUI.Label(new Rect(30, Screen.height - 100, 300, 100), "Photon Beam");

        if (this.mounted && latched_to != null)
		    GUI.Label(new Rect(30, Screen.height - 80, 300, 100), "Piggyback: [Mounted on:" + latched_to.player.name + "]");
        else
            GUI.Label(new Rect(30, Screen.height - 80, 300, 100), "Piggyback: [Not Mounted]");
        GUI.Label(new Rect(30, Screen.height - 60, 300, 100), ability_skill2.ToString());
	}

    public override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
        if (squeek_mount_on != null)
            Destroy(squeek_mount_on.gameObject);
        for (int i = 0; i < PHOTON_BEAM_PARTICLE_COUNT; i++)
            Destroy(latch_beam_particles[i].gameObject);
    }
}
