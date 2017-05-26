using UnityEngine;
using System.Collections;

public class KamSlashView : MonoBehaviour
{
	public void Update()
    {
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                                GetComponent<SpriteRenderer>().color.g,
                                                                GetComponent<SpriteRenderer>().color.b,
                                                                GetComponent<SpriteRenderer>().color.a - 0.12f);
    }
}
