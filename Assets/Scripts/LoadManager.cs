using UnityEngine;
using System.Collections;

/// <summary>
/// Manages everything we want to load up when we're starting a new level.
/// </summary>
public class LoadManager : MonoBehaviour
{
    CameraManager camera_manager;


	void Awake()
    {
        
        camera_manager = gameObject.AddComponent<CameraManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
