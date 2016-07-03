using UnityEngine;
using System.Collections;

public class Damager : NetworkTeam
{
    public float damage = 0;

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
    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (!isServer)
            return;
        if (col.gameObject.tag == "Impassable")
            Destroy(this.gameObject);
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject.GetComponent<Player>().GetTeam() == this.GetTeam())
                return;
            col.gameObject.GetComponent<Player>().ChangeHealth(-damage);
            Destroy(this.gameObject);
        }
    }
}
