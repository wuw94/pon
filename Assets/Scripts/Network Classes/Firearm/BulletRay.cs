using UnityEngine;
using System.Linq;
using System.Collections;

public enum HitType
{
    Nothing,Entity,Wall
}

public class BulletRay
{
    public const int UI = 5;
    public const int DYNAMICLIGHT = 8;
    public const int NETWORKENTITY = 12;
    public const int CHARACTER = 13;
    public const int SHIELD = 14;
    public const int HASHP = 1 << UI | 1 << DYNAMICLIGHT | 1 << NETWORKENTITY | 1 << CHARACTER | 1 << SHIELD;

    public Team team;
    public Vector2 start_point;
    public float angle;
    public float max_distance;
    public RaycastHit2D ray = new RaycastHit2D();
    public HitType hit;

    public BulletRay(Team team, Vector2 start_point, float angle, float max_distance)
    {
        this.team = team;
        this.start_point = start_point;
        this.angle = angle;
        this.max_distance = max_distance;
        CalculateRay();
    }

    private void CalculateRay()
    {
        Quaternion direction = Quaternion.Euler(0, 0, angle);

        hit = HitType.Nothing;

        // Raycast to entities or walls.
        RaycastHit2D[] rays = Physics2D.RaycastAll(this.start_point, (Vector2)(direction * Vector2.up), this.max_distance, HASHP).OrderBy(h => h.distance).ToArray();

        foreach (RaycastHit2D ray_entity in rays)
        {
            int l = ray_entity.transform.gameObject.layer;
            if (l == DYNAMICLIGHT)
            {
                ray = ray_entity;
                hit = HitType.Wall;
                return;
            }
            else if ((l == UI || l == NETWORKENTITY || l == CHARACTER || l == SHIELD) )
            {
                if (ray_entity.transform.GetComponent<NetworkEntity>() != null && ray_entity.transform.GetComponent<NetworkEntity>().GetTeam() != team)
                {
                    ray = ray_entity;
                    hit = HitType.Entity;
                    return;
                }
            }
        }
    }
}
