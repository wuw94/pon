using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Rockjaw : Character
{
    // Characteristics
    public override float max_health { get { return 250; } set { throw new NotImplementedException(); } }
    public override float max_speed { get { return 3.5f; } set { throw new NotImplementedException(); } }

    // Primary Weapon
    private const float _primary_cooldown = 0.9f;

    [SerializeField]
    private Shotgun primary;

    // Skill 1 (Unload)
    private const float _skill1_cooldown = 0.1f;
    private bool _skill1_using = false;

    // Skill 2 (Dash)
    private const float _skill2_cooldown = 6.0f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        ability_primary.SetCooldown(_primary_cooldown);
        ability_reload.SetCooldown(0);
        ability_skill1.SetCooldown(_skill1_cooldown);
        ability_skill1.name = "Unload";
        ability_skill2.SetCooldown(_skill2_cooldown);
        ability_skill2.name = "Dash";
    }

    public override void Passive()
    {
        return;
    }

    // ------------------------------------------------- Shotgun -------------------------------------------------
    public override void PrimaryAttack()
    {
        if (!primary.is_reloading)
            primary.Fire(GetMouseDirection(attacking_offset.position));
        else
            ability_primary.Reset();
    }

    public override void Reload()
    {
        StartCoroutine(primary.Reload());
    }

    // ------------------------------------------------- Unload -------------------------------------------------
    public override void Skill1()
    {
        if (stun_status.IsActive())
        {
            ability_skill1.Reset();
            return;
        }
        if (!_skill1_using)
            StartCoroutine(Unload());
    }

    private IEnumerator Unload()
    {
        float spread = primary.spread;
        primary.spread *= 2; // double the spread
        _skill1_using = true;
        int num_shots = primary.ammunition.current;
        for (int i = 0; i < num_shots; i++)
        {
            PrimaryAttack();
            yield return new WaitForSeconds(0.1f);
        }
        primary.spread = spread; // return to original spread
        _skill1_using = false;
    }

    // ------------------------------------------------- Dash -------------------------------------------------
    public override void Skill2()
    {
        if (root_status.IsActive())
        {
            ability_skill2.Reset();
            return;
        }

        root_status.CmdInflict(0.1f);

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
