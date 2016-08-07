using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class Shotgun : Firearm
{
    [SerializeField]
    private float spread;

    [SerializeField]
    private int num_shots;

    public override void Fire(float angle)
    {
        List<float> angles = new List<float>();
        angles.Add(angle);
        for (int i = 0; i < num_shots; i++)
            angles.Add(angle + Random.Range(-spread, spread));

        FireToward(angles.ToArray());
    }
}
