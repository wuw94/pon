using UnityEngine;
using System.Collections;

/// <summary>
/// Derive from this for a client-calculated object you want to display.
/// These do not have to be networked, and their color is automatically adapted by the origin.
/// </summary>
public class ClientCalculatedView : MonoBehaviour
{
    [HideInInspector]
    public float timeout;
}
