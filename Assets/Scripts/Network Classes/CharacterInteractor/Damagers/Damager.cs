using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Damager : CharacterInteractor
{
    public float damage = 0;

    public override void DoToEnemy(Character c)
    {
        base.DoToEnemy(c);
    }

    public void DamagePlayer(Character c)
    {
        c.ChangeHealth(-damage);
    }

    public void DamagePlayer(Character c, float percentage)
    {
        c.ChangeHealth(-damage * percentage);
    }
}
