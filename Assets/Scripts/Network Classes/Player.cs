using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/* Player.
 * 
 * The Player class is the manager for a player. Each client joining to play
 * has one of these.
 */

 /// <summary>
 /// A class to represent a player. This is not the character itself!
 /// </summary>
public class Player : NetworkBehaviour
{
    public static Player mine;

    public static Player host;

    public Character character;

    [SerializeField]
    private GameObject[] possible_characters;

    [SyncVar(hook = "OnUpdateCharId")]
    private NetworkInstanceId character_id;


    // Stuff to do just to a client player right when it loads
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Player.mine = this;
        if (isServer && isClient)
            host = this;
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
        g.GetComponent<Character>().player_id = this.netId;
    }
    
    [Command]
    public void CmdDestroyCharacter(NetworkInstanceId id)
    {
        Destroy(NetworkServer.FindLocalObject(id));
    }

    private void OnUpdateCharId(NetworkInstanceId id)
    {
        this.character_id = id;
        if (ClientScene.FindLocalObject(character_id) == null)
            this.character = null;
        this.character = ClientScene.FindLocalObject(character_id).GetComponent<Character>();
    }
}
