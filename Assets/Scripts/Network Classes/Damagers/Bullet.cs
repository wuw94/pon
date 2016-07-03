using UnityEngine;
using System.Collections;

public class Bullet : Damager
{
    public Vector2 startpoint;
    public float distance;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (Vector2.Distance(startpoint, new Vector2(transform.position.x, transform.position.y)) > distance)
        {
            Destroy(this.gameObject);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;
        
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject.GetComponent<Player>().GetTeam() == this.GetTeam())
                return;
            col.gameObject.GetComponent<Player>().ChangeHealth(-damage);
            Destroy(this.gameObject);
        }
    }
}
