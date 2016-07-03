using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour
{

    private void Start()
    {
        Destroy(this.gameObject, 1);
    }

	private void FixedUpdate()
    {
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                            GetComponent<SpriteRenderer>().color.g,
                                                            GetComponent<SpriteRenderer>().color.b,
                                                            GetComponent<SpriteRenderer>().color.a - 0.01f);
        transform.localScale = new Vector3(transform.localScale.x * 0.99f, transform.localScale.y, transform.localScale.z);
	}
}
