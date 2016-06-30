using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Punch : Damager
{

    public override void Start()
    {
        base.Start();

        //Destroy(this.gameObject, 1);
    }

    public override void Update()
    {
        base.Update();
        //UpdateColor();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        /*
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
                                                            GetComponent<SpriteRenderer>().color.g,
                                                            GetComponent<SpriteRenderer>().color.b,
                                                            GetComponent<SpriteRenderer>().color.a - 0.2f);
                                                            */

    }

    protected override void UpdateColor()
    {
        if (hasAuthority)
            GetComponent<SpriteRenderer>().color = white;
        else if (GetTeam() == Team.Neutral)
            GetComponent<SpriteRenderer>().color = Color.gray;
        else if (GetTeam() == Player.mine.GetTeam())
            GetComponent<SpriteRenderer>().color = white;
        else if (GetTeam() != Player.mine.GetTeam())
            GetComponent<SpriteRenderer>().color = red;
        else
            GetComponent<SpriteRenderer>().color = Color.yellow;
    }

}
