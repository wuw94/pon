using UnityEngine;
using System.Linq;
using System.Collections;

public enum HitType
{
    Nothing,Entity,Wall
}

public class BulletRay
{
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

        // Raycast to entities first.
        RaycastHit2D[] rays = Physics2D.RaycastAll(this.start_point, (Vector2)(direction * Vector2.up), this.max_distance, 1 << 12).OrderBy(h => h.distance).ToArray();

        foreach (RaycastHit2D ray_entity in rays)
        {
            if (ray_entity.transform.GetComponent<NetworkEntity>().GetTeam() != team)
            {
                ray = ray_entity;
                hit = HitType.Entity;
                return;
            }
        }

        // If we arrived here, we failed to hit an entity. Now we raycast to walls.
        ray = Physics2D.Raycast(this.start_point, (Vector2)(direction * Vector2.up), this.max_distance, 1 << 8);
        if (ray)
            hit = HitType.Wall;
    }
}
