using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    public PlayerInfo player;
    private float _movespeed = 3.0f;

    public GameObject punch;

    private bool can_punch = true;


	private void Update ()
    {
        if (!isLocalPlayer)
            return;
        
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        CheckMovement();
        CheckPunch();
    }


    private void CheckMovement()
    {
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector2.up * _movespeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector2.down * _movespeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector2.left * _movespeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector2.right * _movespeed * Time.deltaTime);
        /*
        if (Input.GetMouseButton(0))
        {
            
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _movespeed * Time.deltaTime);
        }
        */
    }

    IEnumerator PunchCooldown()
    {
        can_punch = false;
        yield return new WaitForSeconds(0.5f);
        can_punch = true;
    }

    private void CheckPunch()
    {
        if (player.on_nucleus)
            return;
        if (Input.GetMouseButton(0) && can_punch)
        {
            StartCoroutine("PunchCooldown");
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Get Angle in Radians
            float AngleRad = Mathf.Atan2(mouse.y - this.transform.position.y, mouse.x - this.transform.position.x);
            // Get Angle in Degrees
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            // Rotate Object
            CmdPunch(Quaternion.Euler(0, 0, AngleDeg - 90));
        }
    }





    [Command]
    void CmdPunch(Quaternion direction)
    {
        GameObject g = (GameObject)Instantiate(punch, transform.position, transform.rotation);
        
        g.transform.rotation = direction;

        g.GetComponent<Rigidbody2D>().velocity = g.transform.up * 20;
        g.GetComponent<Punch>().team = player.team;
        NetworkServer.Spawn(g);
        Destroy(g, 0.02f);
    }



    /// <summary>
    /// Use this to print something on the server's debug. Useful if we want to debug stuff
    /// </summary>
    /// <param name="message"></param>
    [Command]
    public void CmdPrintOnServer(string message)
    {
        Debug.Log(message);
    }
}
