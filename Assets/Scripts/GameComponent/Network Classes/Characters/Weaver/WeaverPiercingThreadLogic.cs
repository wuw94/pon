using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaverPiercingThreadLogic : CharacterInteractor
{
    [SyncVar]
    public NetworkInstanceId owner_id;

    public int damage;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Timeout());
    }
    
    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(0.4f);
        foreach (Character enemy in enemies_held)
            enemy.ChangeHealth(ClientScene.FindLocalObject(owner_id).GetComponent<Character>(), -damage);
        Destroy(this.gameObject);
    }
}