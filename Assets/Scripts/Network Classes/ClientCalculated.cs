using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// A class to hold client-calculated objects.
/// </summary>
public class ClientCalculated : NetworkBehaviour
{
    public float timeout;
    public ClientCalculatedView view;
    public ClientCalculatedLogic logic;

    private NetworkConnection conn;
    
    /// <summary>
    /// Use this function to show the ability locally first.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void ShowLocal(Vector3 position, Quaternion rotation)
    {
        ClientCalculatedView v = Instantiate<ClientCalculatedView>(view);
        v.transform.position = position;
        v.transform.rotation = rotation;
        v.timeout = this.timeout;
    }

    /// <summary>
    /// Spawn the object with Command first before calling this function.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="team"></param>
    /// <param name="conn"></param>
    public void Make(Vector3 position, Quaternion rotation, Team team, NetworkConnection conn)
    {
        this.conn = conn;
        CmdMake(position, rotation, team);
    }

    [Command]
    private void CmdMake(Vector3 position, Quaternion rotation, Team team)
    {
        ClientCalculatedLogic l = Instantiate<ClientCalculatedLogic>(logic);
        l.PreSpawnChangeTeam(team);
        l.transform.position = position;
        l.transform.rotation = rotation;
        l.timeout = this.timeout;
        NetworkServer.SpawnWithClientAuthority(l.gameObject, this.conn);
        l.RpcMakeVisuals();
        Destroy(this.gameObject);
    }
}
