using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

	
	private void FixedUpdate()
    {
        if (target == null)
            return;
        this.transform.position = new Vector3(target.position.x, target.position.y, this.transform.position.z);
        this.transform.rotation = target.rotation;
	}
}
