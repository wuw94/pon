using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Shows our ping, FPS, memory usage, etc.
public class GUIRunDetails : NetworkBehaviour
{
    private Ping my_ping;

    private void Start()
    {
        my_ping = new Ping(MasterServer.ipAddress);
    }

    private void Update()
    {
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 0, 100, 100), "FPS: " + (int)(1.0f / Time.smoothDeltaTime));
        if (my_ping.isDone)
            GUI.Label(new Rect(Screen.width - 150, 20, 100, 100), "Ping: " + my_ping.time + "ms");
        GUI.Label(new Rect(10, 40, 100, 100), "Objects: " + FindObjectsOfType<GameObject>().Length);
        GUI.Label(new Rect(10, 60, 100, 100), "Colliders: " + FindObjectsOfType<Collider2D>().Length);
    }

}
