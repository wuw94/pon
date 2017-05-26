using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// A module is a component you can add to the MapGenerator to generate specific features of a map.
/// </summary>
public abstract class Module : NetworkBehaviour
{
    protected MapGenerator map;
    public bool generate;
    protected Texture2D2 texture;
    private GameObject occluded;

    [SyncVar]
    public bool done = false;

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

    /// <summary>
    /// Called when we want to restart a game with the same map.
    /// </summary>
    public abstract void Reset();


    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!generate)
            return;
        map = FindObjectOfType<MapGenerator>();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        Initialize();
        Debug.Log(name + " took " + stopwatch.ElapsedMilliseconds + "ms to complete.");
        
        done = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        MakeOccludedObject();
        map = FindObjectOfType<MapGenerator>();
        
        if (GetComponent<SpriteRenderer>() != null)
            texture = MapGenerator.GetBlankTexture(map.dimension);
        
        //StartCoroutine(WaitForDone());
    }

    private void MakeOccludedObject()
    {
        this.occluded = new GameObject();
        this.occluded.isStatic = true;
        this.occluded.name = "Occluded";
        this.occluded.transform.position = this.transform.position;
        this.occluded.transform.parent = this.transform;
        this.occluded.layer = 6;
        SpriteRenderer sr = this.occluded.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
        sr.material = this.GetComponent<SpriteRenderer>().material;
    }

    private IEnumerator WaitForDone()
    {
        while (!done)
            yield return null;
        Draw();
        this.GetComponent<SpriteRenderer>().sprite = MapGenerator.ConvertToSprite(texture.natural);
        this.occluded.GetComponent<SpriteRenderer>().sprite = MapGenerator.ConvertToSprite(texture.occluded);

        if (this.GetType() == typeof(BuildingModule))
            Player.mine.CmdDoneGeneratingMap();
    }

    [ClientRpc]
    public void RpcIsDoneDoDraw()
    {
        Draw();
        this.GetComponent<SpriteRenderer>().sprite = MapGenerator.ConvertToSprite(texture.natural);
        this.occluded.GetComponent<SpriteRenderer>().sprite = MapGenerator.ConvertToSprite(texture.occluded);

        if (this.GetType() == typeof(BuildingModule))
            Player.mine.CmdDoneGeneratingMap();
    }
}
