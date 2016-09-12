using UnityEngine;

public class Expand : MonoBehaviour
{
    public float rate;

	private void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x + rate * Time.deltaTime,
                                            transform.localScale.y + rate * Time.deltaTime,
                                            transform.localScale.z);
	}
}
