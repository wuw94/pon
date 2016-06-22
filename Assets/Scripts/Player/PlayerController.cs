using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    public PlayerInfo player;
    private float _movespeed = 3.0f;

    public GameObject punch;




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
        if (Input.GetMouseButton(0))
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _movespeed * Time.deltaTime);
        }
    }

    private void CheckPunch()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdPunch();
        }
    }


    [Command]
    void CmdPunch()
    {
        GameObject g = (GameObject)Instantiate(punch, transform.position, transform.rotation);
        g.GetComponent<Rigidbody2D>().velocity = g.transform.up * 1;
        g.GetComponent<Punch>().team = player.team;
        NetworkServer.Spawn(g);
        Destroy(g, 1.0f);
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
