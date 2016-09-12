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

    public void Shake(float intensity, float duration, Quaternion dir)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(intensity, duration, dir));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration, Quaternion dir)
    {
        Quaternion d = dir;
        float t = duration;
        float i = intensity;
        while (t > 0)
        {
            Vector2 rand_pos = Random.insideUnitCircle * i + (Vector2)(d * Vector2.down * i * 1.2f);
            if (target != null)
            {
                this.transform.position = new Vector3(target.transform.position.x + rand_pos.x,
                                                        target.transform.position.y + rand_pos.y,
                                                        this.transform.position.z);
            }
            t -= Time.deltaTime;
            i -= Time.deltaTime * intensity / duration;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


}
