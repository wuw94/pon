public abstract class Menu
{
    protected const int PG_GROUP_WIDTH = 600;
    protected const int PG_GROUP_HEIGHT = 400;

    protected const int IG_GROUP_WIDTH = 180;
    protected const int IG_GROUP_HEIGHT = 400;

    public string name;

    /// <summary>
    /// What appears when we want to run the GUI of this menu class.
    /// </summary>
    public abstract void RunGUI();

    /// <summary>
    /// What happens when we press ESCAPE when we're using this menu class.
    /// </summary>
    public abstract void Esc();
}