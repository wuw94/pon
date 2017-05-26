using UnityEngine;

public class FadeAway : MonoBehaviour
{
    public float rate;

    private void Update()
    {
        Color c = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, Mathf.Clamp(c.a - rate * Time.deltaTime, 0, 1));
    }
}
