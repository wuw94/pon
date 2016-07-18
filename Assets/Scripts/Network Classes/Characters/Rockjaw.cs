using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Rockjaw : Character
{
    // Characteristics
    public override float max_health { get { return 200; } set { throw new NotImplementedException(); } }
    public override float max_speed { get { return 10.0f; } set { throw new NotImplementedException(); } }

    // Primary Weapon
    private const float max_distance = 10;
    private const float speed = 100;
    private const float damage = 10;
    private const bool damage_fall_off = true;
    private const float _primary_cooldown = 0.2f;
    public GameObject projectile;

    // Skill 1 (Dash)
    private const float _skill1_cooldown = 0.5f;

    // Skill 2 (Unload)
    private const float _skill2_cooldown = 2.0f;





    public override void OnStartClient()
    {
        base.OnStartClient();
        //projectile = Resources.Load<GameObject>("Damagers/Bullet/bullet");
        Revive();
        primary.SetCooldown(_primary_cooldown);
        skill1.SetCooldown(_skill1_cooldown);
        skill2.SetCooldown(_skill2_cooldown);
    }

    public override void Passive()
    {
        return;
    }

    // Shotgun
    public override void PrimaryAttack()
    {
        if (!isLocalPlayer)
            return;

        CmdFireToward(GetMouseDirection());
        for (int i = 0; i < 10; i++)
            CmdFireToward(GetMouseDirection() + UnityEngine.Random.Range(-10,10));

        GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, GetMouseDirection()) * Vector2.down * 10;
    }

    [Command]
    private void CmdFireToward(float angle)
    {
        Quaternion direction = Quaternion.Euler(0, 0, angle);
        GameObject g = (GameObject)Instantiate(projectile, transform.position, direction);
        //RaycastHit2D ray = Physics2D.Raycast(transform.position, (Vector2)(direction * Vector2.up), max_distance, 1 << 8);
        g.GetComponent<Bullet>().Initialize(direction, max_distance, damage, damage_fall_off, speed);

        CmdSpawn(g, this.GetTeam());
    }
    

    // Dash
    public override void Skill1()
    {
        Move(Camera.main.transform.rotation * Vector2.up);
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
            skill1.Reset();
        else
            StartCoroutine(Dash(dir));
        
        /*
        StartCoroutine(Dash(Quaternion.Euler(0, 0, GetMouseDirection()) * Vector2.up));
        */
    }

    private IEnumerator Dash(Vector2 dir)
    {
        can_move = false;
        for (int i = 0; i < 4; i++)
        {
            GetComponent<Rigidbody2D>().velocity = dir * 15;
            yield return new WaitForSeconds(0.02f);
        }
        GetComponent<Rigidbody2D>().velocity = dir * 0.5f;
        can_move = true;
    }

    // Unload
    public override void Skill2()
    {
        StartCoroutine(Unload());
    }

    private IEnumerator Unload()
    {
        for (int i = 0; i < 4; i++)
        {
            PrimaryAttack();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
