using UnityEngine;
using System.Collections;

public class BulletHit : MonoBehaviour
{
    private float expand_rate = 0.1f;

    private void Start()
    {
        StartCoroutine(FadeAway());
        Destroy(this.gameObject, 1);
    }

    private IEnumerator FadeAway()
    {
        while (true)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                                GetComponent<SpriteRenderer>().color.g,
                                                                GetComponent<SpriteRenderer>().color.b,
                                                                GetComponent<SpriteRenderer>().color.a * 0.7f);
            transform.localScale = new Vector3(transform.localScale.x + expand_rate, transform.localScale.y + expand_rate, transform.localScale.z);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
