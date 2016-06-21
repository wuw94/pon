using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PointToMouse : NetworkBehaviour
{
    private void Update()
    {
        if (!isLocalPlayer)
            return;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Get Angle in Radians
        float AngleRad = Mathf.Atan2(mouse.y - this.transform.position.y, mouse.x - this.transform.position.x);
        // Get Angle in Degrees
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        // Rotate Object
        this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
    }
}
