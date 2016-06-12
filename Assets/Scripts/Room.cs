using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    /* Dimensions
     * working_space_x.x gives the left bound of a room
     * working_space_x.y gives the right bound of a room
     * working_space_y.x gives the bottom bound of a room
     * working_space_y.y gives the top bound of a room
     */
    public Vector2 working_space_x;
    public Vector2 working_space_y;

    /* Depth
     * This is the depth of the room. We check it against LevelGenerator.max_depth to make sure we don't make infinite rooms.
     */
    public int depth;

    /* Padding
     * Padding determines the bounds of how small you can make your room.
     * A room cannot be less than padding * padding dimensions.
     */
    private int padding = 2;

    public GameObject room;
    public GameObject wall;
    private GameObject vertical_wall;
    private GameObject horizontal_wall;

    public void setInfo(Vector2 wsx, Vector2 wsy, int d)
    {
        working_space_x = wsx;
        working_space_y = wsy;
        depth = d;

        if (depth < LevelGenerator.max_depth)
        {
            createDividers();
            generateNextRoom();
        }
    }
    
    private void createDividers()
    {
        if (getWidth() > padding * 2)
        {
            vertical_wall = (GameObject)Instantiate(wall, new Vector3(getRandomX(), center().y, 0), Quaternion.identity);
            vertical_wall.transform.localScale = new Vector3(1, getHeight(), 1);
            vertical_wall.name = "Vertical Wall";
            vertical_wall.transform.parent = transform;
        }

        if (getHeight() > padding * 2)
        {
            horizontal_wall = (GameObject)Instantiate(wall, new Vector3(center().x, getRandomY(), 0), Quaternion.identity);
            horizontal_wall.transform.localScale = new Vector3(getWidth(), 1, 1);
            horizontal_wall.name = "Horizontal Wall";
            horizontal_wall.transform.parent = transform;
        }
    }
    
    private void generateNextRoom()
    {
        
        GameObject TL_room = Instantiate(Resources.Load<GameObject>("Prefabs/Room"));
        TL_room.GetComponent<Room>().setInfo(new Vector2(working_space_x.x, vertical_wall.transform.position.x - 0.5f),
                                            new Vector2(horizontal_wall.transform.position.y + 0.5f, working_space_y.y),
                                            depth+1);
        TL_room.name = "TL Room (depth=" + (depth+1).ToString() + ")";
        TL_room.transform.parent = transform;

        
        GameObject TR_room = Instantiate(Resources.Load<GameObject>("Prefabs/Room"));
        TR_room.GetComponent<Room>().setInfo(new Vector2(vertical_wall.transform.position.x + 0.5f, working_space_x.y),
                                            new Vector2(horizontal_wall.transform.position.y + 0.5f, working_space_y.y),
                                            depth + 1);
        TR_room.name = "TR Room (depth=" + (depth + 1).ToString() + ")";
        TR_room.transform.parent = transform;

        GameObject BL_room = Instantiate(Resources.Load<GameObject>("Prefabs/Room"));
        BL_room.GetComponent<Room>().setInfo(new Vector2(working_space_x.x, vertical_wall.transform.position.x - 0.5f),
                                            new Vector2(working_space_y.x, horizontal_wall.transform.position.y - 0.5f),
                                            depth + 1);
        BL_room.name = "BL Room (depth=" + (depth + 1).ToString() + ")";
        BL_room.transform.parent = transform;

        GameObject BR_room = Instantiate(Resources.Load<GameObject>("Prefabs/Room"));
        BR_room.GetComponent<Room>().setInfo(new Vector2(vertical_wall.transform.position.x + 0.5f, working_space_x.y),
                                            new Vector2(working_space_y.x, horizontal_wall.transform.position.y - 0.5f),
                                            depth + 1);
        BR_room.name = "BR Room (depth=" + (depth + 1).ToString() + ")";
        BR_room.transform.parent = transform;
        
    }



    public Vector2 center()
    {
        return new Vector2((working_space_x.x + working_space_x.y) / 2, (working_space_y.x + working_space_y.y) / 2);
    }

    public float getWidth()
    {
        return working_space_x.y - working_space_x.x;
    }

    public float getHeight()
    {
        return working_space_y.y - working_space_y.x;
    }

    private int randomSign()
    {
        return Random.value < 0.5f ? -1 : 1;
    }

    private float getRandomX()
    {
        float to_return = randomSign() * Random.value * (getWidth() / 2 - 2 - padding); // gets us a value from the center that we're sure doesn't exceed padding
        to_return /= LevelGenerator.moderate; // make the value closer to the center
        to_return += center().x; // make the value relative to the center
        to_return = Mathf.Floor(to_return); // floor the value to make sure we have a nice number for room sizes
        to_return += getWidth() % 2 == 0 ? 0.5f : 0; // deal with a problem that occurs between even and odd widths
        return to_return;
    }

    private float getRandomY()
    {
        float to_return = randomSign() * Random.value * (getHeight() / 2 - 2 - padding); // gets us a value from the center that we're sure doesn't exceed padding
        to_return /= LevelGenerator.moderate; // make the value closer to the center
        to_return += center().y; // make the value relative to the center
        to_return = Mathf.Floor(to_return); // floor the value to make sure we have a nice number for room sizes
        to_return += getHeight() % 2 == 0 ? 0.5f : 0; // deal with a problem that occurs between even and odd widths
        return to_return;
    }

}
