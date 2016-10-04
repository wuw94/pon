using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A class to hold client-calculated objects.
/// </summary>
public class ClientCalculated : NetworkBehaviour
{
    public Character owner;
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
        view.transform.position = position;
        view.transform.rotation = rotation;
        view.timeout = this.timeout;
        Instantiate<ClientCalculatedView>(view);
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
        logic.timeout = this.timeout;
        logic.transform.position = position;
        logic.transform.rotation = rotation;
        ClientCalculatedLogic l = Instantiate<ClientCalculatedLogic>(logic);
        l.PreSpawnChangeTeam(team);
        NetworkServer.SpawnWithClientAuthority(l.gameObject, this.conn);
        l.RpcMakeVisuals();
        Destroy(this.gameObject);
    }
}
