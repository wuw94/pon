using System.Collections;
using UnityEngine;

public class SystemObject : MonoBehaviour
{
    public Database.DataSystem dataSystem;
    public UI.UISystem uiSystem;
    private GarbageCollector garbageCollector;
    
    private void Awake()
    {
        dataSystem = new Database.DataSystem(this);
        uiSystem = new UI.UISystem(this);
        garbageCollector = new GarbageCollector();
    }

    private void Update()
    {
        dataSystem.Update();
        uiSystem.Update();
    }

    private void OnGUI()
    {
        dataSystem.OnGUI();
        uiSystem.OnGUI();
        garbageCollector.OnGUI();
    }

    public Coroutine CoroutineStart(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void CoroutineStop(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
}