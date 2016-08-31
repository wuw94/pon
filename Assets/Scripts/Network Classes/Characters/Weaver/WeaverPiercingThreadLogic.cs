using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaverPiercingThreadLogic : ClientCalculatedLogic
{
    public int damage;
    public WeaverPiercingThreadView weaver_piercing_thread_view;


    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Finish());
    }
    
    private IEnumerator Finish()
    {
        yield return new WaitForSeconds(timeout);
        foreach (Character enemy in enemies_held)
            enemy.ChangeHealth(-damage);
        Destroy(this.gameObject);
    }
    
}
