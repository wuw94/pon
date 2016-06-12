using UnityEngine;
using System.Collections;

/*
public class Controllable : UnityEngine.Networking.NetworkBehaviour {
    private float moveSpeed = 2.0f;
	void Start()
    {
	
	}

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }
}
*/
public class Controllable : MonoBehaviour
{
    private float moveSpeed = 1000.0f;
    void Start()
    {

    }

    void Update()
    {
        //Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //targetPos.z = transform.position.z;
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
}
