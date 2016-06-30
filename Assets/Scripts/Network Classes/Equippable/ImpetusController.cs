using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ImpetusController : NetworkTeam
{
    private const int speed = 25;

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
        CheckInputs();
    }

    IEnumerator Cooldown()
    {
        can_use = false;
        yield return new WaitForSeconds(1.0f);
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
            CmdUse(Quaternion.Euler(0, 0, AngleDeg - 90));
        }
    }

    

    [Command]
    void CmdUse(Quaternion direction)
    {
        GameObject g = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
        g.transform.rotation = direction;
        //g.transform.position += g.transform.up * 0;
        g.GetComponent<Rigidbody2D>().velocity = g.transform.up * speed;
        g.GetComponent<Damager>().damage = 70;
        NetworkServer.Spawn(g);
        g.GetComponent<Damager>().ChangeTeam(player.GetTeam());
        Destroy(g, 1);
    }
}
