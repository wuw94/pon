using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : Damager
{
    private bool _damage_fall_off = false;

    private bool _use_trail = true;

    // Where the bullet is created
    private Vector2 _start_point;

    // How far the bullet should travel (predetermined by your weapon and raycasting against walls)
    private float _distance;

    private Vector2 _end_point;

    // What direction the bullet should move in
    private Quaternion _direction;

    // Prefab for creating a bullet trail
    public GameObject bullet_trail;

    public GameObject bullet_hit;

    // The actual trail
    private BulletTrail trail;


    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!isServer)
            return;
        if (Vector2.Distance(_start_point, new Vector2(transform.position.x, transform.position.y)) > _distance)
        {
            OnHitWall();
        }
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;

        if (col.gameObject.tag == "Player")
        {
            if (this.GetTeam() == Team.Neutral || col.gameObject.GetComponent<Player>().GetTeam() == this.GetTeam())
                return;
            if (_use_trail && trail != null)
            {
                trail.distance = Vector2.Distance(col.transform.position, _start_point);
                if (trail.distance == 0)
                    RpcUpdateTrailDistance(0);
                else
                    RpcUpdateTrailDistance(trail.distance);
            }
            if (trail == null)
            {
                RpcUpdateTrailDistance(0);
            }
            OnHitPlayer(col.GetComponent<Player>());
        }
    }

    // only called on the server
    public void SetVars(Quaternion direction, float distance, Vector2 point, float damage, float speed)
    {
        this._start_point = transform.position;
        this._distance = distance;
        this._end_point = point;
        this.transform.rotation = direction;
        this._direction = direction;
        this.damage = damage;
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }

    
    public virtual void OnHitWall()
    {
        RpcMakeHit(_end_point);
    }

    public virtual void OnHitPlayer(Player player)
    {
        if (_damage_fall_off)
            player.ChangeHealth(-damage / Vector2.Distance(player.transform.position, _start_point));
        else
            player.ChangeHealth(-damage);
        Destroy(this.gameObject);
    }


    

    // Code for creating bullet trails
    [ClientRpc]
    public void RpcMakeLine(Vector2 start_point, Quaternion direction, float distance, float speed)
    {
        if (!_use_trail)
            return;
        GameObject g = (GameObject)Instantiate(bullet_trail, start_point, direction);
        trail = g.GetComponent<BulletTrail>();
        trail.source = this;
        trail.distance = distance;
        trail.speed = speed;
    }

    [ClientRpc]
    public void RpcUpdateTrailDistance(float distance)
    {
        if (!_use_trail)
            return;
        trail.distance = distance;
    }

    [ClientRpc]
    public void RpcMakeHit(Vector2 position)
    {
        GameObject g = (GameObject)Instantiate(bullet_hit, position, transform.rotation);
        g.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        Destroy(this.gameObject);
    }

}
