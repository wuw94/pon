using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PortToSpawn : NetworkBehaviour
{
    public Team team;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Character>() != null)
        {
            //col.gameObject.GetComponent<PlayerInfo>().CmdUpdateTeam(team);
            //col.gameObject.GetComponent<PlayerController>().PortToSpawn();
            if (!isServer)
                return;

            col.gameObject.GetComponent<Character>().ChangeTeam(team);
            col.gameObject.GetComponent<Character>().RpcPortToSpawn(team);
        }
    }
}
