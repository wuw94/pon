using UnityEngine;
using System;

public class Pistol : Firearm
{
    public override bool input { get { return Input.GetMouseButton(0); } set { throw new NotImplementedException(); } }
    public override float cooldown { get { return 0.5f; } set { throw new NotImplementedException(); } }

    protected override int speed { get { return 40; } set { throw new NotImplementedException(); } } // 40
    protected override int max_distance { get { return 4; } set { throw new NotImplementedException(); } }
    protected override float damage { get { return 50; } set { throw new NotImplementedException(); } } // 50
    protected override bool damage_fall_off { get { return true; } set { throw new NotImplementedException(); } }


    public override void Use(float angle)
    {
        StartCoroutine(Cooldown());
        CmdFireToward(angle);
    }
}