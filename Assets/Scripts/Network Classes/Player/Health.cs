using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public Player player;
    

    private void Update ()
    {
        transform.localScale = new Vector3(player.GetHealth() / 10.0f, 1, 1);
	}
}
