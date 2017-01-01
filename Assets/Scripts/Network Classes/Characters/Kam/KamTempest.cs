using UnityEngine;
using System.Collections;

public class KamTempest : MonoBehaviour
{
    public void Start()
    {
        Destroy(this.gameObject, 1.0f);
    }
    
    public void Update()
    {
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                                GetComponent<SpriteRenderer>().color.g,
                                                                GetComponent<SpriteRenderer>().color.b,
                                                                GetComponent<SpriteRenderer>().color.a - 0.05f);
    }
}
