using UnityEngine;
using System.Collections;

public class LerpFollow : MonoBehaviour
{
    private const int speed = 100;
    public Transform target;

	private void Update()
    {
        if (target == null)
            return;

        if (Vector2.Distance(this.transform.position, target.transform.position) > 200)
            this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 1, this.transform.position.z);

        this.transform.position = new Vector3(  target.transform.position.x,
                                                target.transform.position.y,
                                                transform.position.z);
	}
}
