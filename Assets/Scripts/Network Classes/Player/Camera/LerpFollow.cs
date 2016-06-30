using UnityEngine;
using System.Collections;

public class LerpFollow : MonoBehaviour
{
    private const int speed = 5;
    public Transform target;

	private void FixedUpdate()
    {
        if (target == null)
            return;

        if (Vector2.Distance(this.transform.position, target.transform.position) > 200)
            this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, this.transform.position.z);

        this.transform.position = new Vector3(  Mathf.Lerp(this.transform.position.x, target.transform.position.x, speed * Time.deltaTime),
                                                Mathf.Lerp(this.transform.position.y, target.transform.position.y, speed * Time.deltaTime),
                                                transform.position.z);
	}
}
