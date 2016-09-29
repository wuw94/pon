using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time;

	private void Start()
    {
        Destroy(this.gameObject, time);
	}
}
