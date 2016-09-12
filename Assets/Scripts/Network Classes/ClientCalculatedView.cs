using UnityEngine;
using System.Collections;

/// <summary>
/// Derive from this for a client-calculated object you want to display.
/// These do not have to be networked, and their color is automatically adapted by the origin.
/// These objects destroy themselves after timeout time is reached.
/// </summary>
public class ClientCalculatedView : MonoBehaviour
{
    [HideInInspector]
    public float timeout;

    public virtual void Start()
    {
        StartCoroutine(Timeout());
    }

    public virtual void Update()
    {

    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(timeout);
        Destroy(this.gameObject);
    }
}
