using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PortToSpawn : NetworkBehaviour
{
    public Team team;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            //col.gameObject.GetComponent<PlayerInfo>().CmdUpdateTeam(team);
            //col.gameObject.GetComponent<PlayerController>().PortToSpawn();
            if (!isServer)
                return;
            col.gameObject.GetComponent<PlayerInfo>().RpcUpdateTeam(team);
            col.gameObject.GetComponent<PlayerInfo>().RpcPortToSpawn();
        }
    }

    [Command]
    public void CmdPrintOnServer(string message)
    {
        Debug.Log(message);
    }
}
