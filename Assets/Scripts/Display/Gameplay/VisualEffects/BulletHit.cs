using UnityEngine;
using System.Collections;

public class BulletHit : MonoBehaviour
{
    public float expand_rate = 0.05f;
    public float fade_away = 0.9f;

    private void Start()
    {
        StartCoroutine(FadeAway());
        Destroy(this.gameObject, 0.7f);
    }

    private IEnumerator FadeAway()
    {
        fade_away = Random.Range(fade_away - 0.05f, fade_away + 0.05f);
        expand_rate = Random.Range(expand_rate - 0.02f, expand_rate);
        while (true)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                                GetComponent<SpriteRenderer>().color.g,
                                                                GetComponent<SpriteRenderer>().color.b,
                                                                GetComponent<SpriteRenderer>().color.a * fade_away);
            transform.localScale = new Vector3(transform.localScale.x + expand_rate, transform.localScale.y + expand_rate, transform.localScale.z);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
