using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class KamWindSlashLogic : NetworkTeam
{
    public KamTempest kam_tempest;
    [HideInInspector]
    public Character owner;
    [HideInInspector]
    public float damage;
    public float speed;

    public override void OnStartServer()
    {
        base.OnStartServer();
        GetComponent<Rigidbody2D>().velocity = (transform.rotation * Vector2.up) * speed;
        Destroy(this.gameObject, 0.5f);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Instantiate(kam_tempest.gameObject, transform.position, transform.rotation);
        GetComponent<Rigidbody2D>().velocity = (transform.rotation * Vector2.up) * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer)
            return;
        if (other.name == "ColliderWall")
            Destroy(this.gameObject);
        if (other.GetComponent<Character>() != null && other.GetComponent<Character>().GetTeam() != owner.GetTeam())
        {
            other.GetComponent<Character>().ChangeHealth(owner, -damage);
            Destroy(this.gameObject);
        }
    }
}
