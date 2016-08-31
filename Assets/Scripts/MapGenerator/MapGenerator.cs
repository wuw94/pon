using UnityEngine;
using UnityEngine.Networking;

public class MapGenerator : NetworkBehaviour
{
    public const int PIXELS_PER_UNIT = 100;
    public const int DRAW_PAD = 1;

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
            NetworkServer.Spawn(Instantiate(module.gameObject));
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
    public static Texture2D2 GetBlankTexture(Vector2 dimensions)
    {
        dimensions += new Vector2(DRAW_PAD * 2, DRAW_PAD * 2);
        Texture2D tex1 = new Texture2D((int)dimensions.x * PIXELS_PER_UNIT, (int)dimensions.y * PIXELS_PER_UNIT);
        for (int x = 0; x < dimensions.x * PIXELS_PER_UNIT; x++)
            for (int y = 0; y < dimensions.y * PIXELS_PER_UNIT; y++)
                tex1.SetPixel(x, y, new Color(0, 0, 0, 0));
        tex1.Apply();

        Texture2D tex2 = new Texture2D((int)dimensions.x * PIXELS_PER_UNIT, (int)dimensions.y * PIXELS_PER_UNIT);
        for (int x = 0; x < dimensions.x * PIXELS_PER_UNIT; x++)
            for (int y = 0; y < dimensions.y * PIXELS_PER_UNIT; y++)
                tex2.SetPixel(x, y, new Color(0, 0, 0, 0));
        tex2.Apply();

        return new Texture2D2(tex1, tex2);
    }

    /// <summary>
    /// Add a new texture on top of the original ref 'texture'.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="to_add"></param>
    public static void AddToTexture(ref Texture2D2 original, Vector2 position, Sprite2 to_add)
    {
        position += new Vector2(DRAW_PAD, DRAW_PAD);

        // Perform operation for natural
        Color[] colors1 = original.natural.GetPixels((int)(position.x * PIXELS_PER_UNIT), (int)(position.y * PIXELS_PER_UNIT), to_add.natural.texture.width, to_add.natural.texture.height);
        Color[] colors2 = to_add.natural.texture.GetPixels();
        Color[] new_colors = new Color[colors1.Length];
        for (int i = 0; i < new_colors.Length; i++)
        {
            float r = Mathf.Clamp(colors1[i].r * (1 - colors2[i].a) + colors2[i].r * colors2[i].a, 0, 1);
            float g = Mathf.Clamp(colors1[i].g * (1 - colors2[i].a) + colors2[i].g * colors2[i].a, 0, 1);
            float b = Mathf.Clamp(colors1[i].b * (1 - colors2[i].a) + colors2[i].b * colors2[i].a, 0, 1);
            float a = Mathf.Clamp(colors1[i].a + colors2[i].a, 0, 1);
            new_colors[i] = new Color(r, g, b, a);
        }
        original.natural.SetPixels((int)(position.x * PIXELS_PER_UNIT), (int)(position.y * PIXELS_PER_UNIT), to_add.natural.texture.width, to_add.natural.texture.height, new_colors);

        // Perform operation for occluded
        colors1 = original.occluded.GetPixels((int)(position.x * PIXELS_PER_UNIT), (int)(position.y * PIXELS_PER_UNIT), to_add.occluded.texture.width, to_add.occluded.texture.height);
        colors2 = to_add.occluded.texture.GetPixels();
        new_colors = new Color[colors1.Length];
        for (int i = 0; i < new_colors.Length; i++)
        {
            float r = Mathf.Clamp(colors1[i].r * (1 - colors2[i].a) + colors2[i].r * colors2[i].a, 0, 1);
            float g = Mathf.Clamp(colors1[i].g * (1 - colors2[i].a) + colors2[i].g * colors2[i].a, 0, 1);
            float b = Mathf.Clamp(colors1[i].b * (1 - colors2[i].a) + colors2[i].b * colors2[i].a, 0, 1);
            float a = Mathf.Clamp(colors1[i].a + colors2[i].a, 0, 1);
            new_colors[i] = new Color(r, g, b, a);
        }
        original.occluded.SetPixels((int)(position.x * PIXELS_PER_UNIT), (int)(position.y * PIXELS_PER_UNIT), to_add.occluded.texture.width, to_add.occluded.texture.height, new_colors);
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
