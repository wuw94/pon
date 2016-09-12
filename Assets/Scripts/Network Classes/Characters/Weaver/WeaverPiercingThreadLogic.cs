using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaverPiercingThreadLogic : ClientCalculatedLogic
{
    public int damage;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Timeout());
    }
    
    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(timeout);
        foreach (Character enemy in enemies_held)
            enemy.ChangeHealth(-damage);
        Destroy(this.gameObject);
    }
}