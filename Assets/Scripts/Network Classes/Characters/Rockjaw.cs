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
    private const float _primary_cooldown = 0.9f;

    [SerializeField]
    private Firearm primary_weapon;

    // Skill 1 (Unload)
    private const float _skill1_cooldown = 10.0f;

    // Skill 2 (Dash)
    private const float _skill2_cooldown = 7.0f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        //projectile = Resources.Load<GameObject>("Damagers/Bullet/bullet");
        Revive();
        primary.SetCooldown(_primary_cooldown);
        skill1.SetCooldown(_skill1_cooldown);
        skill1.name = "Unload";
        skill2.SetCooldown(_skill2_cooldown);
        skill2.name = "Blitz";
    }

    public override void Passive()
    {
        return;
    }

    // Shotgun
    public override void PrimaryAttack()
    {
        if (!hasAuthority)
            return;

        primary_weapon.Fire(GetMouseDirection());
        GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, GetMouseDirection()) * Vector2.down * 5;
    }
    
    // Dash
    public override void Skill1()
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

    // Unload
    public override void Skill2()
    {
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
            StartCoroutine(Dash());
    }

    

    private IEnumerator Dash()
    {
        can_move = false;
        //Vector2 dir = Quaternion.Euler(0, 0, GetMouseDirection()) * Vector2.up;
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

            GetComponent<Rigidbody2D>().velocity = dir * 25;
            yield return new WaitForSeconds(0.02f);
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        can_move = true;
    }
}
