using UnityEngine;
using System.Collections;

public class LerpFollow : MonoBehaviour
{
    public Transform target;
	void Update ()
    {
        if (target == null)
            return;
        this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
	}
}
