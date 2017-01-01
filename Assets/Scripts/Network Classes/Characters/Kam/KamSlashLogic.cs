using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class KamSlashLogic : CharacterInteractor
{
    [SyncVar]
    public NetworkInstanceId owner_id;

    public float damage;
    // A list of all the enemies we've already hit
    private List<Character> enemies_hit = new List<Character>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Timeout());
    }

    public override void OnEnemyEnter(Character c)
    {
        base.OnEnemyEnter(c);
        if (!enemies_hit.Contains(c))
        {
            c.ChangeHealth(ClientScene.FindLocalObject(owner_id).GetComponent<Character>(), -damage);
            enemies_hit.Add(c);
        }
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }
}
