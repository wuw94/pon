using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct StatusAlteration
{
    public float _time_stamp;

    public StatusAlteration(float current_time)
    {
        _time_stamp = current_time;
    }

    /// <summary>
    /// Returns whether this status alteration is currently occurring.
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return Time.time < _time_stamp;
    }

    /// <summary>
    /// Inflicts this status alteration for a given amount of time.
    /// If the current status alteration should continue for longer than the to-be-inflicted,
    /// the effect isn't extended.
    /// </summary>
    /// <param name="duration"></param>
    public void Inflict(float duration)
    {
        if (_time_stamp < Time.time + duration)
            _time_stamp = Time.time + duration;
    }

    /// <summary>
    /// Removes this status alteration.
    /// </summary>
    public void Remove()
    {
        _time_stamp = Time.time;
    }
}
