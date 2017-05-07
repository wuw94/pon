using System;

public class GarbageCollector
{
    public GarbageCollector()
    {
    }

    public void OnGUI()
    {
        UnityEngine.GUI.Label(new UnityEngine.Rect(0, 0, 1000, 20), string.Format("{0:0.##}", GC.GetTotalMemory(true) / 1000000.0f) + "MB");
    }
}