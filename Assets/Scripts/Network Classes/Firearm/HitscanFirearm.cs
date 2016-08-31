using UnityEngine;
using UnityEngine.Networking;

public abstract class HitscanFirearm : Firearm
{
    protected void FireToward(float[] angles)
    {
        foreach (float angle in angles)
        {
            Quaternion direction = Quaternion.Euler(0, 0, angle);

            BulletRay bullet_ray = new BulletRay(GetComponent<Character>().GetTeam(), owner.attacking_offset.position, angle, max_distance);


            // Make Trail
            GameObject g = (GameObject)Instantiate(projectile.GetComponent<Bullet>().trail_prefab, owner.attacking_offset.position, direction);
            g.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;


            if (bullet_ray.hit == HitType.Nothing) // if we hit nothing
            {
                g.transform.localScale = new Vector3(g.transform.localScale.x, max_distance, g.transform.localScale.z);
            }
            else if (bullet_ray.hit == HitType.Entity) // if we hit an entity
            {
                g.transform.localScale = new Vector3(g.transform.localScale.x, bullet_ray.ray.distance, g.transform.localScale.z);
                Instantiate(projectile.GetComponent<Bullet>().hit_prefab, bullet_ray.ray.point, direction);
                CmdDamage(bullet_ray.ray.transform.GetComponent<NetworkIdentity>().netId);
            }
            else if (bullet_ray.hit == HitType.Wall) //  if we hit a wall
            {
                g.transform.localScale = new Vector3(g.transform.localScale.x, bullet_ray.ray.distance, g.transform.localScale.z);
                Instantiate(projectile.GetComponent<Bullet>().hit_prefab, bullet_ray.ray.point, direction);
            }
        }
        CmdFireListAngles(angles);
    }


    [Command]
    private void CmdFireAngle(float angle, bool did_hit)
    {
        GameObject g = (GameObject)Instantiate(projectile);
        Bullet b = g.GetComponent<Bullet>();
        b.Initialize(owner.attacking_offset.position, angle, max_distance, did_hit);

        g.GetComponent<NetworkTeam>().PreSpawnChangeTeam(GetComponent<Character>().GetTeam());
        NetworkServer.SpawnWithClientAuthority(g, GetComponent<Character>().player.gameObject);
        g.GetComponent<Bullet>().RpcMakeTrail(GetComponent<Character>().GetTeam());
    }


    [Command]
    private void CmdFireListAngles(float[] angles)
    {
        GameObject g = (GameObject)Instantiate(projectile);
        Bullet b = g.GetComponent<Bullet>();
        b.Initialize(owner.attacking_offset.position, angles, max_distance);

        g.GetComponent<NetworkTeam>().PreSpawnChangeTeam(GetComponent<Character>().GetTeam());
        NetworkServer.SpawnWithClientAuthority(g, GetComponent<Character>().player.gameObject);
        g.GetComponent<Bullet>().RpcMakeTrail(GetComponent<Character>().GetTeam());
    }

    [Command]
    private void CmdFireAt(NetworkInstanceId hit)
    {
        NetworkEntity ne = ClientScene.FindLocalObject(hit).GetComponent<NetworkEntity>();

        float AngleRad = Mathf.Atan2(ne.transform.position.y - this.transform.position.y, ne.transform.position.x - this.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        float angle = AngleDeg - 90;
        Quaternion direction = Quaternion.Euler(0, 0, angle);

        GameObject g = (GameObject)Instantiate(projectile, transform.position, direction);
        Bullet b = g.GetComponent<Bullet>();
        b.Initialize(transform.position, angle, max_distance, true);

        g.GetComponent<NetworkTeam>().PreSpawnChangeTeam(GetComponent<Character>().GetTeam());

        NetworkServer.SpawnWithClientAuthority(g, GetComponent<Character>().player.gameObject);
        g.GetComponent<Bullet>().RpcMakeTrail(GetComponent<Character>().GetTeam());
    }

    [Command]
    private void CmdDamage(NetworkInstanceId hit)
    {
        NetworkEntity ne = ClientScene.FindLocalObject(hit).GetComponent<NetworkEntity>();
        float distance = Vector2.Distance(owner.attacking_offset.position, ne.transform.position);
        if (fall_off_type == FalloffType.Hard)
            ne.ChangeHealth(-damage * (1 - distance / max_distance));
        else if (fall_off_type == FalloffType.Medium)
            ne.ChangeHealth(-damage * (1 - distance / max_distance) * 0.5f - damage * 0.5f);
        else if (fall_off_type == FalloffType.None)
            ne.ChangeHealth(-damage);
    }
}
