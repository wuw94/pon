using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Shows our ping, FPS, memory usage, etc.
public class GUIRunDetails : NetworkBehaviour
{
    
    private Ping _my_ping;

    private void Start()
    {
        _my_ping = new Ping(FindObjectOfType<Server>().serverBindAddress);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 100, 0, 100, 100), "FPS: " + (int)(1.0f / Time.smoothDeltaTime));
        GUI.Label(new Rect(Screen.width - 100, 20, 100, 100), "Ping: " + _my_ping.time + "ms");
        GUI.Label(new Rect(Screen.width - 100, 40, 100, 100), "Colliders: " + FindObjectsOfType<Collider2D>().Length);
    }
}
