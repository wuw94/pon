using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PreGameMenu : MonoBehaviour
{
    private const string typeName = "Lost";
    private const string gameName = "zjiozejio";

    private void StartServer()
    {
        Network.InitializeServer(4, 7777, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
    }

    private HostData[] hostList;

    private void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
    }

    private void OnStartHost()
    {
        Debug.Log("Hosted");
    }

    private void OnGUI()
    {
        int height = Screen.height;
        int width = Screen.width;
        if (GUI.Button(new Rect(width / 2, height / 2, 100, 20), "Host a game"))
        {
            FindObjectOfType<NetworkManager>().StartHost();
            //MasterServer.RegisterHost(typeName, gameName);

        }
        if (hostList != null)
        {
            for (int i = 0; i < hostList.Length; i++)
            {
                if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                    JoinServer(hostList[i]);
            }
        }
    }

    private void Update()
    {
        RefreshHostList();
    }
}
