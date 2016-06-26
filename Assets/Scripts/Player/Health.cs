using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public PlayerInfo player;
    

    private void Update ()
    {
        transform.localScale = new Vector3(player.current_health / 10, 1, 1);
	}
}
