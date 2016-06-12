using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{
    public static int max_depth = 4; //how many times we want to recursively create rooms
    public static float moderate = 1.7f; // how moderate (opposite of extreme) we want each room to be
    public static float width = 100; // this is the size of the interior, without regard to the wall thickness
    public static float height = 100;
    public static Vector2 center = new Vector2(0, 0);

    public GameObject room;
    public GameObject wall;
    

    void Start()
    {
        setBounds();
        beginRoomGeneration();
    }

    private void setBounds()
    {
        GameObject bounds = new GameObject("Bounds");
        bounds.transform.parent = transform;
        GameObject left_wall = (GameObject)Instantiate(wall, new Vector3(center.x - width / 2 - 0.5f, center.y - 0.5f, 0), Quaternion.identity);
        left_wall.transform.localScale = new Vector3(1, height + 1, 1);
        left_wall.name = "Left Wall";
        left_wall.transform.parent = bounds.transform;
        GameObject right_wall = (GameObject)Instantiate(wall, new Vector3(center.x + width / 2 + 0.5f, center.y + 0.5f, 0), Quaternion.identity);
        right_wall.transform.localScale = new Vector3(1, height + 1, 1);
        right_wall.name = "Right Wall";
        right_wall.transform.parent = bounds.transform;
        GameObject top_wall = (GameObject)Instantiate(wall, new Vector3(center.x - 0.5f, center.y + height / 2 + 0.5f, 0), Quaternion.identity);
        top_wall.transform.localScale = new Vector3(width + 1, 1, 1);
        top_wall.name = "Top Wall";
        top_wall.transform.parent = bounds.transform;
        GameObject bottom_wall = (GameObject)Instantiate(wall, new Vector3(center.x / 2 + 0.5f, center.y - height / 2 - 0.5f, 0), Quaternion.identity);
        bottom_wall.transform.localScale = new Vector3(width + 1, 1, 1);
        bottom_wall.name = "Bottom Wall";
        bottom_wall.transform.parent = bounds.transform;
    }

    private void beginRoomGeneration()
    {
        GameObject room = Instantiate(Resources.Load<GameObject>("Prefabs/Room"));
        room.GetComponent<Room>().setInfo(  new Vector2(center.x - width / 2, center.x + width / 2),
                                            new Vector2(center.y - height / 2, center.y + height / 2),
                                            0   );
        room.name = "Room (depth=0)";
        room.transform.parent = transform;
    }
}
