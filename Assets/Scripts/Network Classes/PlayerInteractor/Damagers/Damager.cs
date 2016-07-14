using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Damager : PlayerInteractor
{
    public float damage = 0;

    public override void DoToEnemy(Player p)
    {
        base.DoToEnemy(p);
    }

    public void DamagePlayer(Player p)
    {
        p.ChangeHealth(-damage);
    }

    public void DamagePlayer(Player p, float percentage)
    {
        p.ChangeHealth(-damage * percentage);
    }
}
