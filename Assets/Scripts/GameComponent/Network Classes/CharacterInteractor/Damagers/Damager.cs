public class Damager : CharacterInteractor
{
    public float damage = 0;

    public override void DoToEnemy(Character c)
    {
        base.DoToEnemy(c);
    }

    public void DamageCharacter(Character c)
    {
        //c.ChangeHealth(-damage);
    }

    public void DamageCharacter(Character c, float percentage)
    {
        //c.ChangeHealth(-damage * percentage);
    }
}