using UnityEngine;
using System.Collections;

public class BulletTrail : MonoBehaviour
{
    public float speed;
    public float distance;
    public bool main;
    public Bullet source;

    private void Start()
    {
        if (main)
            transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z); // distance * 5
        StartCoroutine(FadeAway());
        Destroy(this.gameObject, 2);
    }

    private IEnumerator FadeAway()
    {
        float fade_away = Random.Range(0.015f, 0.03f);
        while (true)
        {
            if (source != null && source.GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color = new Color(source.GetComponent<SpriteRenderer>().color.r,
                                                                    source.GetComponent<SpriteRenderer>().color.g,
                                                                    source.GetComponent<SpriteRenderer>().color.b,
                                                                    GetComponent<SpriteRenderer>().color.a - fade_away);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                                    GetComponent<SpriteRenderer>().color.g,
                                                                    GetComponent<SpriteRenderer>().color.b,
                                                                    GetComponent<SpriteRenderer>().color.a - fade_away);
            }
            if (main)
                transform.localScale = new Vector3(transform.localScale.x * 0.99f, Mathf.Clamp(transform.localScale.y + speed / 60, 0, distance), transform.localScale.z);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
