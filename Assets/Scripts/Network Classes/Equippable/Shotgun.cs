using UnityEngine;
using System;

public class Shotgun : Firearm
{
    public override bool input { get { return Input.GetMouseButton(0); } set { throw new NotImplementedException(); } }
    public override float cooldown { get { return 0.9f; } set { throw new NotImplementedException(); } }

    protected override int speed { get { return 30; } set { throw new NotImplementedException(); } }
    protected override int max_distance { get { return 3; } set { throw new NotImplementedException(); } }
    protected override float damage { get { return 10; } set { throw new NotImplementedException(); } }
    protected override bool damage_fall_off { get { return true; } set { throw new NotImplementedException(); } }


    public override void Use(float angle)
    {
        StartCoroutine(Cooldown());
        for (int i = -14; i <= 14; i += 2)
        {
            CmdFireToward(angle + i);
        }
    }
}
