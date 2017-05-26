using UnityEngine;
using AmplifyBloom;
using System.Collections;

public class BloomAdjust : MonoBehaviour
{
    public float time;

	void Start()
    {
        StartCoroutine(ChangeBloomValues());
	}
	
	// Update is called once per frame
	void Update ()
    {
        //GetComponent<AmplifyBloomEffect>().OverallIntensity = Random.Range(0.3f, 0.5f);
        //GetComponent<AmplifyBloomEffect>().OverallThreshold = Random.Range(0.4f, 0.5f);
	}

    IEnumerator ChangeBloomValues()
    {
        while (true)
        {
            GetComponent<AmplifyBloomEffect>().OverallThreshold = Random.Range(0.45f, 0.5f);
            yield return new WaitForSeconds(time);
        }
    }
}
