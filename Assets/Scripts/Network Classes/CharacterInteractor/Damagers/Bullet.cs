using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class Bullet : NetworkTeam
{
    [SyncVar]
    public Vector2 _start_point; // Where the bullet is created

    public SyncListFloat _angles = new SyncListFloat();

    [SyncVar]
    private float _max_distance; // How far the bullet can travel
    
    // Prefab for creating a bullet trail
    public GameObject trail_prefab;

    public GameObject hit_prefab;

    public void Initialize(Vector2 start_point, float[] angles, float max_distance)
    {
        this._start_point = start_point;
        foreach (float angle in angles)
            this._angles.Add(angle);
        this._max_distance = max_distance;
    }

    public void Initialize(Vector2 start_point, float angle, float max_distance, bool did_hit)
    {
        this._start_point = start_point;
        this._angles.Add(angle);
        this._max_distance = max_distance;
    }
    
    [ClientRpc]
    public void RpcMakeTrail(Team team)
    {
        if (!hasAuthority)
        {
            foreach (float angle in _angles)
            {
                Quaternion direction = Quaternion.Euler(0, 0, angle);

                BulletRay bullet_ray = new BulletRay(team, this._start_point, angle, this._max_distance);

                GameObject g = (GameObject)Instantiate(trail_prefab, this._start_point, direction);
                g.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;

                if (bullet_ray.hit == HitType.Nothing)
                    g.transform.localScale = new Vector3(g.transform.localScale.x, this._max_distance, g.transform.localScale.z);
                else
                {
                    g.transform.localScale = new Vector3(g.transform.localScale.x, bullet_ray.ray.distance, g.transform.localScale.z);

                    GameObject hit_obj = (GameObject)Instantiate(hit_prefab, this._start_point + (Vector2)(Quaternion.Euler(0, 0, angle) * Vector2.up) * bullet_ray.ray.distance, Quaternion.Euler(0, 0, angle));
                    hit_obj.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
                }
            }
        }
        Destroy(this.gameObject);
    }
}
