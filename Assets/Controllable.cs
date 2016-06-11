using UnityEngine;
using System.Collections;

public class Controllable : MonoBehaviour {
    private float moveSpeed = 2.0f;
	void Start()
    {
	
	}

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }
}
