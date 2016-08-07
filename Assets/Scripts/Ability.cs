using UnityEngine;
using System.Collections;

public struct InputType
{
    public bool mouse_l;
    public bool mouse_r;
    public KeyCode key;
}

public class Ability
{
    public string name = "";
    private InputType input;
    private float _time_stamp;
    private float _cooldown = 0;

    public Ability(bool l, bool r)
    {
        _time_stamp = Time.time;
        this.input.mouse_l = l;
        this.input.mouse_r = r;
    }

    public Ability(KeyCode k)
    {
        _time_stamp = Time.time;
        this.input.key = k;
    }
    
    public void SetCooldown(float c)
    {
        this._cooldown = c;
    }

    public bool Pressed()
    {
        if (input.mouse_l)
            return Input.GetMouseButton(0);
        else if (input.mouse_r)
            return Input.GetMouseButton(2);
        else
            return Input.GetKey(input.key);
    }

    public bool Ready()
    {
        return _time_stamp <= Time.time;
    }
    
    public float Remaining()
    {
        if (!Ready())
            return _time_stamp - Time.time;
        return 0;
    }

    public void Use()
    {
        _time_stamp = Time.time + _cooldown;
    }

    public void Reset()
    {
        _time_stamp = Time.time;
    }

    public override string ToString()
    {
        if (Ready())
            return name + ": [Ready]";
        return name + ": [" + (int)Remaining() + "]";
    }
}
