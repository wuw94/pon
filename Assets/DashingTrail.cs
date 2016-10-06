using UnityEngine;
using System.Collections;

public class DashingTrail : MonoBehaviour
{
	public Character owner;
	public float destroy_after;
	public float fadeaway;

	private float _disappear_speed;

	public TrailRenderer trail;

	private void Start()
	{
		Destroy(this.gameObject, destroy_after);
	}

	private void Update()
	{
		if (owner != null)
			transform.position = owner.transform.position;
		_disappear_speed += fadeaway * Time.deltaTime;
		trail.startWidth = Mathf.Clamp(trail.startWidth - _disappear_speed, 0,1);
		trail.endWidth = Mathf.Clamp(trail.endWidth - _disappear_speed, 0, 1);
	}
}
