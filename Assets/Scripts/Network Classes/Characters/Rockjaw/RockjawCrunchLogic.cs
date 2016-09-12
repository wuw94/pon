using UnityEngine;
using System.Collections;

public class RockjawCrunchLogic : ClientCalculatedLogic
{
    public float damage;
    public float stun_duration;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Timeout());
    }

    public override void OnEnemyEnter(Character c)
    {
        base.OnEnemyEnter(c);
        c.CmdInflictStun(stun_duration);
        c.ChangeHealth(-damage);
        c.RpcPortToPosition(this.transform.position);
    }

    private IEnumerator Timeout()
    {
        while (stun_duration > 0)
        {
            stun_duration -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}