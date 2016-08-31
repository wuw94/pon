using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletTrail : NetworkBehaviour
{
    public RaycastHit2D ray;
    public float distance;

    public float fade_range_min;
    public float fade_range_max;

    private void Start()
    {
        StartCoroutine(FadeAway());
        Destroy(this.gameObject, 0.7f);
    }

    private IEnumerator FadeAway()
    {
        float fade_away = Random.Range(fade_range_min, fade_range_max);
        while (true)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                                GetComponent<SpriteRenderer>().color.g,
                                                                GetComponent<SpriteRenderer>().color.b,
                                                                GetComponent<SpriteRenderer>().color.a - fade_away);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
