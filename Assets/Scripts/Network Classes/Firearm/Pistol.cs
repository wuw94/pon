public class Pistol : HitscanFirearm
{
    public override void Fire(float angle)
    {
        if (CheckAmmo())
        {
            FireToward(new float[] { angle });
            ammunition.current--;
        }

        if (ammunition.IsEmpty())
        {
            StartCoroutine(Reload());
        }
    }

    public override string ToString()
    {
        if (is_reloading)
            return "Pistol: " + ammunition.current + "/" + ammunition.max + " [Reloading]";
        return "Pistol: " + ammunition.current + "/" + ammunition.max;
    }
}
