using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RoomFadeAway : NetworkBehaviour
{
    public float color = 0;
    public GameObject NoLight;

    private void Start()
    {
        StartCoroutine(Fade());
    }
    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player" && col.GetComponent<Player>().isLocalPlayer)
        {
            color = 0.5f;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player" && col.GetComponent<Player>().isLocalPlayer)
        {
            color = 0.5f;
        }
    }

    IEnumerator Fade()
    {
        while (true)
        {
            if (color > 0)
            {
                color -= 0.001f;
                NoLight.GetComponent<SpriteRenderer>().color = new Color(color, color, color, 1);
            }
            else
            {
                color = 0;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
