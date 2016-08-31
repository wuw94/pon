using UnityEngine;
using System.Collections.Generic;

public class Shotgun : HitscanFirearm
{
    [SerializeField]
    public float spread;

    [SerializeField]
    public int num_shots;

    public override void Fire(float angle)
    {
        if (CheckAmmo())
        {
            List<float> angles = new List<float>();
            angles.Add(angle);
            for (int i = 0; i < num_shots; i++)
                angles.Add(angle + Random.Range(-spread, spread));

            FireToward(angles.ToArray());
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
            return "Shotgun: " + ammunition.current + "/" + ammunition.max + " [Reloading]";
        return "Shotgun: " + ammunition.current + "/" + ammunition.max;
    }
}
