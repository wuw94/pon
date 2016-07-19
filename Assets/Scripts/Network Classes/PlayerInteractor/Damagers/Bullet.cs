using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Bullet : Damager
{
    // Raycasting for accurate bullet logic
    private RaycastHit2D ray;

    [SyncVar]
    private Vector2 _start_point; // Where the bullet is created

    [SyncVar]
    private Quaternion _direction; // What direction the bullet should move in

    [SyncVar]
    private float _max_distance; // How far the bullet can travel

    [SyncVar]
    private bool _damage_fall_off = false; // Does the bullet damage fall off?

    [SyncVar]
    private float _speed;

    [SyncVar]
    private bool _use_trail = true;

    [SyncVar]
    private bool _has_hit = false;

    // Prefab for creating a bullet trail
    public GameObject bullet_trail;

    public GameObject bullet_hit;

    // The actual trail
    private BulletTrail trail;


    public void Initialize(Quaternion direction, float max_distance, float damage, bool damage_fall_off, float speed)
    {
        this._start_point = transform.position;
        this._direction = direction;
        this._max_distance = max_distance;
        this.damage = damage;
        this._damage_fall_off = damage_fall_off;
        this._speed = speed;

        this.ray = Physics2D.Raycast(transform.position, (Vector2)(direction * Vector2.up), max_distance, 1 << 8);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        transform.rotation = _direction;
        GetComponent<Rigidbody2D>().velocity = transform.up * _speed;
        MakeLine();
    }

    public override void Update()
    {
        base.Update();
        if (!isServer)
            return;

        if (_has_hit)
            return;
        if (Vector2.Distance(_start_point, new Vector2(transform.position.x, transform.position.y)) > _max_distance)
            OnReachedMaxDistance();
        else if (ray && Vector2.Distance(_start_point, new Vector2(transform.position.x, transform.position.y)) > ray.distance)
            OnHitWall();
    }

    public override void DoToEnemy(Character c)
    {
        RaycastHit2D ray_to_player;
        Physics2DExtension.Raycast(c.GetComponent<Collider2D>(), new Ray2D(_start_point, _direction * Vector2.up), out ray_to_player, _max_distance);

        if (ray_to_player)
        {
            ray = ray_to_player;
            OnHitEnemy();
        }


        // do the damage
        if (_damage_fall_off)
            if (_max_distance - Vector2.Distance(c.transform.position, _start_point) > 0)
                DamagePlayer(c, (_max_distance - Vector2.Distance(c.transform.position, _start_point)) / _max_distance);
        else
            DamagePlayer(c);
        damage = 0; // set it to zero so it doesn't damage multiple things
    }

    
    public virtual void OnReachedMaxDistance()
    {
        DestroyThis();
    }

    public virtual void OnHitWall()
    {
        _has_hit = true;
        RpcUpdateRay(ray.distance);
        RpcOnHit(ray.point);
    }

    public virtual void OnHitEnemy()
    {
        _has_hit = true;
        RpcUpdateRay(ray.distance);
        RpcOnHit(ray.point);
    }

    
    
    [ClientRpc]
    private void RpcOnHit(Vector2 position)
    {
        GameObject g = (GameObject)Instantiate(bullet_hit, position, transform.rotation);
        g.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        DestroyThis();
    }

    private void DestroyThis()
    {
        if (!isServer)
            return;
        Destroy(this.gameObject);
    }
    

    // Trail Related Code

    public void MakeLine()
    {
        if (!_use_trail)
            return;
        GameObject g = (GameObject)Instantiate(bullet_trail, this._start_point, this._direction);
        trail = g.GetComponent<BulletTrail>();
        trail.source = this;
        if (ray)
            trail.distance = Mathf.Clamp(ray.distance, 0, this._max_distance);
        else
            trail.distance = this._max_distance;
        trail.speed = this._speed;
    }

    [ClientRpc]
    public void RpcUpdateRay(float r_dist)
    {
        if (_use_trail && trail != null)
            trail.distance = r_dist;
        this.ray.distance = r_dist;
    }

}
