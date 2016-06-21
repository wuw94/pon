using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Punch : NetworkBehaviour
{
    [SyncVar]
    public Team team;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject.GetComponent<PlayerInfo>().team == this.team)
                return;
            col.gameObject.GetComponent<PlayerInfo>().TakeDamage(10);
            Destroy(this.gameObject);
        }
    }
	
}
