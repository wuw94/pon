using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RockjawCrunchLogic : CharacterInteractor
{
    [SyncVar]
    public NetworkInstanceId owner_id;

    public float damage;
    public float stun_duration;
    public float damage_occur;
    private Character character_held;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Timeout());
        StartCoroutine(WaitForDamage());
    }

    public override void OnEnemyEnter(Character c)
    {
        base.OnEnemyEnter(c);
        if (character_held == null)
            character_held = c;
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

    private IEnumerator WaitForDamage()
    {
        Character source = ClientScene.FindLocalObject(owner_id).GetComponent<Character>();
        while (damage_occur > 0)
        {
            damage_occur -= Time.deltaTime;
            if (character_held != null)
            {
                character_held.CmdInflictStun(stun_duration);
                character_held.RpcPortToPosition(this.transform.position);
            }
            yield return null;
        }
        if (character_held != null)
            character_held.ChangeHealth(source, -damage);
    }
}