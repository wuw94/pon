using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdenwardBashLogic : ClientCalculatedLogic
{
    public float damage;
    public float dmg_timer;
    private List<Character> enemies_hit = new List<Character>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Timeout());
    }

    public override void Update()
    {
        base.Update();
        dmg_timer -= Time.deltaTime;
    }

    public override void OnEnemyEnter(Character c)
    {
        base.OnEnemyEnter(c);
        if (dmg_timer > 0 && !enemies_hit.Contains(c))
        {
            c.ChangeHealth(client_calculated.owner.player, -damage);
            enemies_hit.Add(c);
        }
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(timeout);
        Destroy(this.gameObject);
    }
}
