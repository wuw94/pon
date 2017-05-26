using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GeneratorTester : NetworkManager
{
    float fps;
    float target_size = 10;
    UsageInfo grid_info;
    Vector2 mouse_pos_world;
    Vector2 mouse_pos_screen;

    private void Start()
    {
        StartHost();
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 150, 0, 100, 100), "FPS: " + fps);
        GUI.Label(new Rect(mouse_pos_screen + new Vector2(5,-15), new Vector2(200, 20)), "(" + (int)mouse_pos_world.x + "," + (int)mouse_pos_world.y + ") - " + grid_info);
    }

    public void Update()
    {
        UpdateFPS();
        UpdateViewPort();
        UpdateMouseInfo();
    }

    private void UpdateFPS()
    {
        fps = (int)(1.0f / Time.smoothDeltaTime);
    }

    private void UpdateViewPort()
    {
        UpdateZoom();
        UpdateCameraMove();
        //in_viewport = Camera.main.pixelRect.Contains(Input.mousePosition);
    }

    private void UpdateZoom()
    {
        target_size += -Input.GetAxis("Mouse ScrollWheel") * target_size * 3;
        target_size = Mathf.Clamp(target_size, 0.1f, 30);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, target_size, Time.deltaTime * 20);
    }

    private void UpdateCameraMove()
    {
        if (Input.GetKey(KeyCode.W))
            Move(Vector2.up * Camera.main.orthographicSize / 100);
        if (Input.GetKey(KeyCode.A))
            Move(Vector2.left * Camera.main.orthographicSize / 100);
        if (Input.GetKey(KeyCode.S))
            Move(Vector2.down * Camera.main.orthographicSize / 100);
        if (Input.GetKey(KeyCode.D))
            Move(Vector2.right * Camera.main.orthographicSize / 100);
    }

    private void Move(Vector2 offset)
    {
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + offset.x,
                                                        Camera.main.transform.position.y + offset.y,
                                                        Camera.main.transform.position.z);
    }

    private void UpdateMouseInfo()
    {
        mouse_pos_world = new Vector2(Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x),
                                        Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        mouse_pos_screen = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        grid_info = FindObjectOfType<MapGenerator>().usage_chart.Info(mouse_pos_world);
    }
}