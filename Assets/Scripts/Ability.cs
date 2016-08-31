using UnityEngine;
using System.Collections;

public struct InputType
{
    public bool mouse_l;
    public bool mouse_r;
    public KeyCode key;
    public bool continuous;
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
        this.input.continuous = true;
    }

    public Ability(KeyCode k, bool continuous)
    {
        _time_stamp = Time.time;
        this.input.key = k;
        this.input.continuous = continuous;
    }
    
    public void SetCooldown(float c)
    {
        this._cooldown = c;
    }

    /// <summary>
    /// Returns whether the hotkey for this ability was pressed in this frame.
    /// </summary>
    /// <returns></returns>
    public bool Pressed()
    {
        if (input.mouse_l)
            return Input.GetMouseButton(0);
        else if (input.mouse_r)
            return Input.GetMouseButton(2);
        else if (input.continuous)
            return Input.GetKey(input.key);
        else
            return Input.GetKeyDown(input.key);
    }

    /// <summary>
    /// Returns whether the ability is ready, i.e. cooldown is 0.
    /// </summary>
    /// <returns></returns>
    public bool Ready()
    {
        return _time_stamp <= Time.time;
    }
    
    /// <summary>
    /// Returns the remaining cooldown for this ability.
    /// </summary>
    /// <returns></returns>
    public float Remaining()
    {
        if (!Ready())
            return _time_stamp - Time.time;
        return 0;
    }

    /// <summary>
    /// Uses this ability, i.e. starting the cooldown.
    /// </summary>
    public void Use()
    {
        _time_stamp = Time.time + _cooldown;
    }

    /// <summary>
    /// Resets the ability, i.e. reseting its cooldown.
    /// </summary>
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
