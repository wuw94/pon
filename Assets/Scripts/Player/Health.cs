using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public PlayerInfo player;
    
    void Update ()
    {
        transform.localScale = new Vector3(player.currentHealth / 10, 1, 1);
	}
}
