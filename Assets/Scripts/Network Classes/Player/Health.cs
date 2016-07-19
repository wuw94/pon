using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public Character character;
    

    private void Update ()
    {
        transform.localScale = new Vector3(character.GetHealth() / 10.0f, 1, 1);
	}
}
