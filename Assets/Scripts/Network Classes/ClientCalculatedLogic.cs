using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Derive from this for the logic of a client-calculated object.
/// Stuff that happens in this class must happen in the server.
/// </summary>
public class ClientCalculatedLogic : CharacterInteractor
{
    [HideInInspector]
    public float timeout;

    [SerializeField]
    private ClientCalculated client_calculated;
    
    [ClientRpc]
    public void RpcMakeVisuals()
    {
        if (!hasAuthority)
        {
            ClientCalculatedView v = Instantiate<ClientCalculatedView>(client_calculated.view);
            v.transform.position = this.transform.position;
            v.transform.rotation = this.transform.rotation;
            v.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        }
    }
}
