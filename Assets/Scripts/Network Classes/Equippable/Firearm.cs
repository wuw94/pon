using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class Firearm : NetworkSpawner
{
    private bool can_use = true;

    public abstract bool input { get; set; }
    public abstract float cooldown { get; set; }

    protected abstract int speed { get; set; }
    protected abstract int max_distance { get; set; }
    protected abstract float damage { get; set; }
    protected abstract bool damage_fall_off { get; set; }

    public Player player;
    public GameObject bullet;


    public void FixedUpdate()
    {
        if (player.IsDead())
            return;
        if (!isLocalPlayer)
            return;
        if (player.GetTeam() == Team.Neutral)
            return;
        CheckInputs();
    }

    public IEnumerator Cooldown()
    {
        can_use = false;
        yield return new WaitForSeconds(cooldown);
        can_use = true;
    }

    protected virtual void CheckInputs()
    {
        if (input && can_use)
        {
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Get Angle in Radians
            float AngleRad = Mathf.Atan2(mouse.y - this.transform.position.y, mouse.x - this.transform.position.x);
            // Get Angle in Degrees
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            Use(AngleDeg - 90);
        }
    }

    public abstract void Use(float angle);

    [Command]
    protected void CmdFireToward(float angle)
    {
        Quaternion direction = Quaternion.Euler(0, 0, angle);

        GameObject g = (GameObject)Instantiate(bullet, transform.position, direction);
        RaycastHit2D ray = Physics2D.Raycast(transform.position, (Vector2)(direction * Vector2.up), max_distance, 1 << 8);
        //g.GetComponent<Bullet>().SetRay(ray);
        g.GetComponent<Bullet>().Initialize(direction, max_distance, damage, damage_fall_off, speed);
        
        CmdSpawn(g, player.GetTeam());
    }
}
