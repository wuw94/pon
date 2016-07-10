using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ImpetusController : NetworkTeam
{
    private const int speed = 40;//50;
    private const int max_distance = 8;
    private const int damage = 70;
    private const float cooldown = 0.0f;


    public Player player;
    public GameObject bullet;
    private bool can_use = true;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player.IsDead())
            return;
        if (!isLocalPlayer)
            return;
        if (player.GetTeam() == Team.Neutral)
            return;
        CheckInputs();
    }

    IEnumerator Cooldown()
    {
        can_use = false;
        yield return new WaitForSeconds(cooldown);
        can_use = true;
    }

    private void CheckInputs()
    {
        if (Input.GetMouseButtonDown(0) && can_use)
        {
            StartCoroutine(Cooldown());
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Get Angle in Radians
            float AngleRad = Mathf.Atan2(mouse.y - this.transform.position.y, mouse.x - this.transform.position.x);
            // Get Angle in Degrees
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            // Rotate Object
            RaycastHit2D hit = Physics2D.Raycast(transform.position, mouse - new Vector2(transform.position.x, transform.position.y), 1000, 1<<8);

            CmdUse(Quaternion.Euler(0, 0, AngleDeg - 90), hit.distance, hit.point);
        }
    }

    

    [Command]
    void CmdUse(Quaternion direction, float distance, Vector2 point)
    {
        GameObject g = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
        g.GetComponent<Collider2D>().enabled = false;
        g.GetComponent<Bullet>().SetVars(direction, distance > max_distance ? max_distance : distance, point, damage, speed);
        NetworkServer.Spawn(g);
        g.GetComponent<Damager>().ChangeTeam(player.GetTeam()); // this must be called after spawn to update color for other players to see.
        g.GetComponent<Bullet>().RpcMakeLine(transform.position, direction, distance > max_distance ? max_distance : distance, speed);
        g.GetComponent<Collider2D>().enabled = true;
    }
}
