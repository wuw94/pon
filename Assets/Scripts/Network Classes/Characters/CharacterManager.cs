using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : NetworkBehaviour
{
    [SyncVar]
    private int _current_index;

    private List<Character> _my_characters = new List<Character>();

	public void Start ()
    {
        Rockjaw c1 = gameObject.GetComponent<Rockjaw>();
        _my_characters.Add(c1);
	}
	
	void Update ()
    {
        if (!isLocalPlayer)
            return;
	    if (Input.GetKeyDown(KeyCode.Delete))
            SwitchCharacter();
	}

    private void SwitchCharacter()
    {
        
    }

    public Character GetCurrentCharacter()
    {
        return _my_characters[_current_index];
    }

    public void ChangeTeam(Team t)
    {
        foreach (Character c in _my_characters)
            c.ChangeTeam(t);
    }
}
