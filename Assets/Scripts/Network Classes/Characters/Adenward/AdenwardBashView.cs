using UnityEngine;

public class AdenwardBashView : MonoBehaviour
{
    private float scale = 0.02f;
    // Fadeaway
	public void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x + scale,
                                            transform.localScale.y + scale,
                                            transform.localScale.z + scale);
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                            GetComponent<SpriteRenderer>().color.g,
                                                            GetComponent<SpriteRenderer>().color.b,
                                                            GetComponent<SpriteRenderer>().color.a - 0.04f);
	}
}
