using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/* Player.
 * 
 * The Player class is the manager for a player. Each client joining to play
 * has one of these.
 */
public class Player : NetworkBehaviour
{
    public static Player mine;

    public Character character
    {
        get
        {
            if (ClientScene.FindLocalObject(character_id) == null)
                return null;
            return ClientScene.FindLocalObject(character_id).GetComponent<Character>();
        }
    }

    [SerializeField]
    private GameObject[] possible_characters;

    [SyncVar]
    private NetworkInstanceId character_id;


    // Stuff to do just to a client player right when it loads
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Player.mine = this;
        CmdMakeCharacter(0);
    }

    public void OnGUI()
    {
        if (!isLocalPlayer)
            return;
        if (character == null)
            return;
        if (character.GetTeam() != Team.Neutral)
            return;
        GUI.Label(new Rect(Screen.width / 2 - 100, 20, 300, 20), "Choose your Character");
        for (int i = 0; i < possible_characters.Length; i++)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 50, 80 + 20 * i, 100, 20), possible_characters[i].name))
            {
                CmdDestroyCharacter(character_id);
                CmdMakeCharacter(i);
            }
        }
    }

    [Command]
    public void CmdMakeCharacter(int index)
    {
        GameObject g = Instantiate<GameObject>(possible_characters[index]);
        g.transform.position = transform.position;
        NetworkServer.SpawnWithClientAuthority(g, this.gameObject);
        character_id = g.GetComponent<NetworkBehaviour>().netId;
    }
    
    [Command]
    public void CmdDestroyCharacter(NetworkInstanceId id)
    {
        Destroy(NetworkServer.FindLocalObject(id));
    }
}
