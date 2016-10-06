using UnityEngine;
using UnityEngine.Networking;

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

    [SyncVar]
    public bool is_host;

	[SyncVar(hook = "OnUpdateCharId")]
	private NetworkInstanceId character_id;

	/// <summary>
	/// The character object being controlled by this player.
	/// </summary>
    public Character character;
	
    [SyncVar]
    public Team selected_team = Team.Neutral;


	[SyncVar(hook = "OnUpdatePlayerName")]
	private string _player_name;

    [SyncVar]
    public bool done_generating_map = false;

	[SyncVar]
	public int selected_character = 0;

    [SerializeField]
    public GameObject[] possible_characters;

    [SyncVar]
    public bool can_choose_character = true;

    public override void OnStartClient()
    {
        base.OnStartClient();
        name = _player_name;
    }

    // Stuff to do just to a client player right when it loads
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetName(Settings.PLAYER_NAME);
        Player.mine = this;
        if (isServer && isClient)
            is_host = true;
        MakeCamera();
        
    }

	// Switch your character to the one in the new index
    public void SwitchCharacter(int change_to)
    {
		can_choose_character = false;
        CmdDestroyCharacter(character_id);
        CmdMakeCharacter(change_to);
    }

    public void MakeCamera()
    {
        Instantiate(Resources.Load("Camera/Main Camera"));
    }

    
    [ClientRpc]
    public void RpcBeginSequence()
    {
        if (this == Player.mine)
            BeginSequence();
    }

    public void BeginSequence()
    {
        Menu.current = MenuPage.IG_Gameplay;
        Destroy(FindObjectOfType<Camera>().gameObject);
        CmdMakeCharacter(selected_character);
    }

    [Command]
    public void CmdChangeSelectedTeam(Team team)
    {
        this.selected_team = team;
    }

    [Command]
    public void CmdChangeSelectedCharacter(int change_to)
    {
        this.selected_character = change_to;
    }

    [Command]
    public void CmdNextCharacter()
    {
        if (this.selected_character < possible_characters.Length - 1)
            this.selected_character++;
    }

    [Command]
    public void CmdPreviousCharacter()
    {
        if (this.selected_character > 0)
            this.selected_character--;
    }

    [Command]
    public void CmdDoneGeneratingMap()
    {
        done_generating_map = true;
    }

    [Command]
    public void CmdMakeCharacter(int index)
    {
        GameObject g = Instantiate<GameObject>(possible_characters[index]);
        g.transform.position = transform.position;
        NetworkServer.SpawnWithClientAuthority(g, this.gameObject);
        g.GetComponent<Character>().ChangeTeam(selected_team);
        g.GetComponent<Character>().RpcPortToSpawn(selected_team);
        character_id = g.GetComponent<NetworkBehaviour>().netId;
        g.GetComponent<Character>().player_id = this.netId;
        can_choose_character = false;
        can_choose_character = true;
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
        else
            this.character = ClientScene.FindLocalObject(character_id).GetComponent<Character>();
    }

    [Command]
    public void CmdSetName(string player_name)
    {
        this._player_name = player_name;
    }

	private void OnUpdatePlayerName(string new_name)
	{
		this._player_name = new_name;
		this.name = new_name;
	}
}
