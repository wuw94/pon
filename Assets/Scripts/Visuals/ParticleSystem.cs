using UnityEngine;
using System.Collections;

public class ParticleSystem : MonoBehaviour
{
    public NetworkTeam source;
    public GameObject particle;


    private void Start()
    {
        StartCoroutine(generateParticles());
    }

    private IEnumerator generateParticles()
    {
        while (true)
        {
            if (source.GetTeam() != Team.Neutral)
            {
                GameObject g = (GameObject)Instantiate(particle, this.transform.position, this.transform.rotation);
                g.GetComponent<SpriteRenderer>().color = source.GetComponent<SpriteRenderer>().color;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
