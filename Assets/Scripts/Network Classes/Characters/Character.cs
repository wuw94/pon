using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class Character : NetworkEntity
{
    private DynamicLight vision;
    private Camera main_camera;


    public abstract float max_speed { get; set; }
    protected bool can_move = true;
    protected Ability primary;
    protected Ability skill1;
    protected Ability skill2;
    
    [SyncVar]
    public bool active = false;

    // ------------------------------------------------- Unity Overrides -------------------------------------------------

    public override void OnStartClient()
    {
        base.OnStartClient();
        primary = new Ability(true, false);
        skill1 = new Ability(KeyCode.LeftShift);
        skill2 = new Ability(KeyCode.Space);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AdjustLayer();
        LocalCamera();
        LocalVision();
    }

    public override void FixedUpdate()
    {
        if (isServer)
            Passive();
        if (hasAuthority)
        {
            FaceMouse();
            CheckMovementInputs();
            CheckSkillInputs();
        }
    }

    // ------------------------------------------------- Setup -------------------------------------------------

    private void AdjustLayer()
    {
        this.gameObject.layer = 5;
        this.GetComponentInChildren<Health>().gameObject.layer = 5;
    }

    private void LocalCamera()
    {
        if (FindObjectOfType<Camera>() == null)
            Instantiate(Resources.Load<GameObject>("Camera/Camera (View Under)"));
        Camera.main.GetComponent<LerpFollow>().target = this.transform;
        main_camera = Camera.main;
    }

    private void LocalVision()
    {
        GameObject g = Instantiate(Resources.Load<GameObject>("Player/Vision"));
        vision = g.GetComponent<DynamicLight>();
        g.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, g.transform.position.z);
        g.transform.parent = this.transform;
        RefreshVision();
    }

    public void RefreshVision()
    {
        vision.Rebuild();
    }

    // ------------------------------------------------- Check Inputs -------------------------------------------------

    private void CheckMovementInputs()
    {
        if (Input.GetKey(KeyCode.W))
            Move(Camera.main.transform.rotation * Vector2.up);
        if (Input.GetKey(KeyCode.S))
            Move(Camera.main.transform.rotation * Vector2.down);
        if (Input.GetKey(KeyCode.A))
            Move(Camera.main.transform.rotation * Vector2.left);
        if (Input.GetKey(KeyCode.D))
            Move(Camera.main.transform.rotation * Vector2.right);
    }

    private void CheckSkillInputs()
    {
        if (primary.Pressed() && primary.Ready())
        {
            primary.Use();
            PrimaryAttack();
        }
        if (skill1.Pressed() && skill1.Ready())
        {
            skill1.Use();
            Skill1();
        }
        if (skill2.Pressed() && skill2.Ready())
        {
            skill2.Use();
            Skill2();
        }
    }

    public virtual void Move(Vector2 dir)
    {
        if (!can_move)
            return;
        Vector2 v = GetComponent<Rigidbody2D>().velocity;

        if (v.x > -max_speed && v.x < max_speed)
            v += dir;
        if (v.y > -max_speed && v.y < max_speed)
            v += dir;

        GetComponent<Rigidbody2D>().velocity = v;
        //transform.Translate(dir * max_speed * Time.deltaTime);
    }

    public abstract void Passive();
    public abstract void PrimaryAttack();
    public abstract void Skill1();
    public abstract void Skill2();

    // ------------------------------------------------- Helper Functions -------------------------------------------------

    protected float GetMouseDirection()
    {
        if (Camera.main == null)
            return 0;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float AngleRad = Mathf.Atan2(mouse.y - this.transform.position.y, mouse.x - this.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        return AngleDeg - 90;
    }

    public void FaceMouse()
    {
        transform.rotation = Quaternion.Euler(0, 0, GetMouseDirection());
    }

    public override void Dead()
    {
        StartCoroutine(RespawnProcess());
    }

    IEnumerator RespawnProcess()
    {
        yield return new WaitForSeconds(5);
        RpcPortToSpawn(GetTeam());
        Revive();
    }






    // ------------------------------------------------- Network Sending -------------------------------------------------

    [Command]
    protected void CmdSpawn(GameObject g, Team t)
    {
        if (g.GetComponent<NetworkTeam>() == null)
        {
            return;
        }
        g.GetComponent<NetworkTeam>().PreSpawnChangeTeam(t);
        NetworkServer.Spawn(g);

        // Known Errors: HandleTransform no gameObject when object is destroyed by server
        // Only happens when command is called from the client
        // https://issuetracker.unity3d.com/issues/handletransform-no-gameobject-log-error-message-after-destroying-a-networked-object
        // Uncomment this once we know the bug is fixed:
        //g.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }


    [ClientRpc]
    public void RpcPortToSpawn(Team t)
    {
        if (!hasAuthority)
            return;

        Level level = FindObjectOfType<Level>();
        if (t == Team.A)
            transform.position = level.SpawnA + new Vector2(1, 1);
        if (t == Team.B)
            transform.position = level.SpawnB + new Vector2(1, 1);
    }
}
