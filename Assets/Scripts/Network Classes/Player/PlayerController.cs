using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : Player
{
    private float _movespeed = 2.3f;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (!isLocalPlayer)
            return;
    }

    public override void FixedUpdate()
    {
        // Running on all computers (server and client)
        if (IsDead())
            return;

        // Are you a server
        if (isServer)
            RegenHealth(0.2f); // Don't do this if you're not a server


        
        if (!isLocalPlayer) // Exit if you're not the computer running this Player
            return;

        
        CheckMovement();
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }


    private void CheckMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * _movespeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * _movespeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * _movespeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * _movespeed * Time.deltaTime);
        }
    }
    

    private void RegenHealth(float n)
    {
        ChangeHealth(n);
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
