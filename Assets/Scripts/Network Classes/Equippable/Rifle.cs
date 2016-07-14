using UnityEngine;
using System;

public class Rifle : Firearm
{
    public override bool input { get { return Input.GetMouseButton(0); } set { throw new NotImplementedException(); } }
    public override float cooldown { get { return 2.0f; } set { throw new NotImplementedException(); } }

    protected override int speed { get { return 50; } set { throw new NotImplementedException(); } }
    protected override int max_distance { get { return 8; } set { throw new NotImplementedException(); } }
    protected override float damage { get { return 90; } set { throw new NotImplementedException(); } }
    protected override bool damage_fall_off { get { return false; } set { throw new NotImplementedException(); } }

    protected override void CheckInputs()
    {
        base.CheckInputs();
    }

    public override void Use(float angle)
    {
        StartCoroutine(Cooldown());
        CmdFireToward(angle);
    }
}
