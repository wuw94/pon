using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MapGenerator : NetworkBehaviour
{
    public const int pixels_per_unit = 100;


    public Point dimension = new Point(50, 25);

    public UsageChart usage_chart;

    public Module[] modules;

    public void Awake()
    {
        usage_chart = new UsageChart(dimension);
    }


    public override void OnStartServer()
    {
        base.OnStartServer();
        foreach (Module module in modules)
            NetworkServer.Spawn(module.gameObject);
    }

    public GameObject Make(string name)
    {
        GameObject go = new GameObject();
        go.name = name;
        GameObject occlude = new GameObject();
        occlude.name = "Occlude";
        occlude.transform.parent = go.transform;
        return go;
    }

    /// <summary>
    /// Gets a blank texture the size of our world. Pixels per unit 100
    /// </summary>
    /// <returns></returns>
    public static Texture2D GetBlankTexture(Vector2 dimensions)
    {
        dimensions += new Vector2(2, 2);
        Texture2D tex = new Texture2D((int)dimensions.x * pixels_per_unit, (int)dimensions.y * pixels_per_unit);
        for (int x = 0; x < dimensions.x * pixels_per_unit; x++)
            for (int y = 0; y < dimensions.y * pixels_per_unit; y++)
                tex.SetPixel(x, y, new Color(0, 0, 0, 0));
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// Add a new texture on top of the original ref 'texture'.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="to_add"></param>
    public static void AddToTexture(ref Texture2D original, Vector2 position, Texture2D to_add)
    {
        position += new Vector2(1, 1);
        Color[] colors1 = original.GetPixels((int)(position.x * pixels_per_unit), (int)(position.y * pixels_per_unit), to_add.width, to_add.height);
        Color[] colors2 = to_add.GetPixels();
        Color[] new_colors = new Color[colors1.Length];
        for (int i = 0; i < new_colors.Length; i++)
        {
            float r = Mathf.Clamp(colors1[i].r * (1 - colors2[i].a) + colors2[i].r * colors2[i].a, 0, 1);
            float g = Mathf.Clamp(colors1[i].g * (1 - colors2[i].a) + colors2[i].g * colors2[i].a, 0, 1);
            float b = Mathf.Clamp(colors1[i].b * (1 - colors2[i].a) + colors2[i].b * colors2[i].a, 0, 1);
            float a = Mathf.Clamp(colors1[i].a + colors2[i].a, 0, 1);
            new_colors[i] = new Color(r, g, b, a);
        }
        original.SetPixels((int)(position.x * pixels_per_unit), (int)(position.y * pixels_per_unit), to_add.width, to_add.height, new_colors);
    }

    /// <summary>
    /// Converts a texture to a sprite, pivoted at the bottom left corner.
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public static Sprite ConvertToSprite(Texture2D texture)
    {
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
