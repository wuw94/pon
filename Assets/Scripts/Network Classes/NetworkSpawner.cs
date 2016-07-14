using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkSpawner : NetworkBehaviour
{
    /// <summary>
    /// When we spawn an object with a team we need to make sure the object actually becomes a certain team
    /// </summary>
    /// <param name="g"></param>
    [Command]
    protected void CmdSpawn(GameObject g, Team t)
    {
        if (g.GetComponent<NetworkTeam>() == null)
        {
            return;
        }
        g.GetComponent<NetworkTeam>().PreSpawnChangeTeam(t);
        NetworkServer.Spawn(g);
        
        

        // Known Errors: HandleTransform no gameObject when object is destroyed by server
        // Only happens when command is called from the client
        // https://issuetracker.unity3d.com/issues/handletransform-no-gameobject-log-error-message-after-destroying-a-networked-object
        // Uncomment this once we know the bug is fixed:
        //g.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }
}
