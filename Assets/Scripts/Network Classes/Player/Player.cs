using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Player : NetworkTeam
{
    public static Player mine;
    public CharacterManager character_manager;
    public DynamicLight vision;
    public Camera player_camera;
    

    // Stuff to do just to a client player right when it loads
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Player.mine = this;

        AdjustLayer();
        LocalCamera();
        LocalVision();
    }

    public override void Update()
    {
        base.Update();
        if (!isLocalPlayer)
            return;
        FaceMouse();
    }

    protected override void OnTeamChanged()
    {
        base.OnTeamChanged();
        character_manager.ChangeTeam(this.GetTeam());
    }

    private void AdjustLayer()
    {
        this.gameObject.layer = 5;
        this.gameObject.GetComponentInChildren<Health>().gameObject.layer = 5;
    }

    private void LocalCamera()
    {
        Instantiate(Resources.Load<GameObject>("Camera/Camera (View Under)"));
        Camera.main.GetComponent<LerpFollow>().target = this.transform;
        player_camera = Camera.main;
    }

    private void LocalVision()
    {
        GameObject g = Instantiate(Resources.Load<GameObject>("Player/Vision"));
        vision = g.GetComponent<DynamicLight>();
        g.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, g.transform.position.z);
        g.transform.rotation = this.transform.rotation;
        g.transform.parent = this.transform;
    }

    public void RefreshVision()
    {
        FindObjectOfType<DynamicLight>().Rebuild();
    }

    public void FaceMouse()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float AngleRad = Mathf.Atan2(mouse.y - this.transform.position.y, mouse.x - this.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
    }
}
