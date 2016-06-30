using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TeamSelect : NetworkBehaviour
{
    public GameObject TeamA;
    public GameObject TeamB;

	private void Start()
    {
        GameObject tA = Instantiate<GameObject>(TeamA);
        GameObject tB = Instantiate<GameObject>(TeamB);

        tA.transform.parent = this.transform;
        tB.transform.parent = this.transform;

        NetworkServer.Spawn(tA);
        NetworkServer.Spawn(tB);
	}
}
