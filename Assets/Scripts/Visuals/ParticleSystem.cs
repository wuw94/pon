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
                Bullet b = GetComponent<Bullet>();
                if (Vector2.Distance(b.startpoint, new Vector2(transform.position.x, transform.position.y)) <= b.distance)
                {
                    GameObject g = (GameObject)Instantiate(particle, this.transform.position, this.transform.rotation);
                    g.GetComponent<SpriteRenderer>().color = source.GetComponent<SpriteRenderer>().color;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
