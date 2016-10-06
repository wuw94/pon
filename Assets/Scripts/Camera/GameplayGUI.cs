using UnityEngine;
using System.Collections;

public class GameplayGUI : MonoBehaviour
{
    public static GameplayGUI singleton;

    public ExpirationStack<string> kill_feed = new ExpirationStack<string>(5);

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
        // Kill Feed
        ExpirationStack<string>.StackContent[] contents = kill_feed.GetContents().ToArray();
        for (int i = 0; i < contents.Length; i++)
            GUI.Label(new Rect(Screen.width - 300, 20 + i * 20, 300, 100), contents[i].element);
    }
}
