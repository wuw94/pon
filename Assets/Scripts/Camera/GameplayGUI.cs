using UnityEngine;
using System.Collections;

public class GameplayGUI : MonoBehaviour
{
    public GUISkin kill_feed_skin;
    public static GameplayGUI singleton;

    public ExpirationQueue<string> kill_feed = new ExpirationQueue<string>(5);

    private void Awake()
    {
        GameplayGUI.singleton = this;
    }


    private void Update()
    {
        kill_feed.Update();
    }

    private void OnGUI()
    {
        GUI.skin = kill_feed_skin;
        // Kill Feed
        ExpirationQueue<string>.ExpirationQueueElement[] contents = kill_feed.GetContents().ToArray();
        for (int i = 0; i < contents.Length; i++)
            GUI.Label(new Rect(Screen.width - 310, 10 + i * 20, 300, 20), contents[i].element);
    }
}
