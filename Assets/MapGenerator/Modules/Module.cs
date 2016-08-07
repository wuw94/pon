using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A module is a component you can add to the MapGenerator to generate specific features of a map.
/// </summary>
public abstract class Module : NetworkBehaviour
{
    public bool generate;
    protected Texture2D texture;
    protected MapGenerator map;

    [SyncVar(hook = "OnDone")]
    protected bool done = false;

    /// <summary>
    /// Called immediately upon creation of module, and only on the server.
    /// Use this for module generation.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Called when Initialize() is done on this module.
    /// Use this for module drawing.
    /// </summary>
    public abstract void Draw();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!generate)
            return;
        map = FindObjectOfType<MapGenerator>();
        if (isServer)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Initialize();
            Debug.Log(name + " took " + stopwatch.ElapsedMilliseconds + "ms to complete.");
        }
        if (GetComponent<SpriteRenderer>() != null)
            texture = MapGenerator.GetBlankTexture(map.dimension);
        if (done)
            Draw();
        else
            done = true;
    }

    public void OnDone(bool set_to)
    {
        done = set_to;
        Draw();
    }
}
