using UnityEngine;

public class SystemMediator : MonoBehaviour
{
    public Data.DataSystem dataSystem;
    public UI.UISystem uiSystem;

    private void Awake()
    {
        if (FindObjectsOfType<SystemMediator>().Length == 1) // If this is the only one.
            DontDestroyOnLoad(this);
        else
            Destroy(gameObject);
    }
}