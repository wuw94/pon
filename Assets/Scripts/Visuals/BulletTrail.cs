using UnityEngine;
using System.Collections;

public class BulletTrail : MonoBehaviour
{
    public float speed;
    public float distance;
    public Bullet source;

    private void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z); // distance * 5
        StartCoroutine(FadeAway());
        Destroy(this.gameObject, 1);
    }

    private IEnumerator FadeAway()
    {
        while (true)
        {
            if (source != null)
            {
                GetComponent<SpriteRenderer>().color = new Color(source.GetComponent<SpriteRenderer>().color.r,
                                                                    source.GetComponent<SpriteRenderer>().color.g,
                                                                    source.GetComponent<SpriteRenderer>().color.b,
                                                                    GetComponent<SpriteRenderer>().color.a - 0.05f);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                                    GetComponent<SpriteRenderer>().color.g,
                                                                    GetComponent<SpriteRenderer>().color.b,
                                                                    GetComponent<SpriteRenderer>().color.a - 0.05f);
            }
            transform.localScale = new Vector3(transform.localScale.x * 0.99f, Mathf.Clamp(transform.localScale.y + speed / 60, 0, distance), transform.localScale.z);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
