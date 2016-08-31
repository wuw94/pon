using UnityEngine;
using System.Collections;

public class WeaverPiercingThreadView : ClientCalculatedView
{
    public void Start()
    {
        StartCoroutine(Finish());
    }

    public void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x / 1.03f, transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator Finish()
    {
        yield return new WaitForSeconds(timeout);
        Destroy(this.gameObject);
    }

}
