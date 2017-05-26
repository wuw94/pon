using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// Class for managing a list of multipliers and durations. This is for movement speed buffs.
/// </summary>
public class MultiplicativeMultipliersList
{
    private const float MIN = 0;
    private const float MAX = 3;
    public struct MultiplierElement
    {
        public float magnitude;
        public float timestamp;
        public AbilityInfo ability_info;

        /// <summary>
        /// Magnitude: the multiplier magnitude.
        /// Timestamp: When this element was added.
        /// Source Name: Source name of ability
        /// Source Owner: Owner of this ability, as a NETID
        /// </summary>
        /// <param name="magnitude"></param>
        /// <param name="timestamp"></param>
        /// <param name="source_name"></param>
        /// <param name="source_owner"></param>
        public MultiplierElement(float magnitude, float timestamp, AbilityInfo ability_info)
        {
            this.magnitude = magnitude;
            this.timestamp = timestamp;
            this.ability_info = ability_info;
        }
    }

    private List<MultiplierElement> _list_multipliers = new List<MultiplierElement>();
    private float _total = 1;

    public MultiplicativeMultipliersList()
    {
    }

    public void Add(float multiplier, float duration, AbilityInfo ability_info)
    {
        for (int i = 0; i < _list_multipliers.Count; i++)
        {
            if (_list_multipliers[i].ability_info.name == ability_info.name && _list_multipliers[i].ability_info.source == ability_info.source)
            {
                _list_multipliers[i] = new MultiplierElement(multiplier, Time.time + duration, ability_info);
                if (_list_multipliers[i].magnitude != multiplier)
                {
                    UpdateTotal();
                }
                return;
            }
        }
        _list_multipliers.Add(new MultiplierElement(multiplier, Time.time + duration, ability_info));
        UpdateTotal();
    }

    public float Total()
    {
        return _total;
    }

    private void UpdateTotal()
    {
        float new_total = 1;
        foreach (MultiplierElement me in _list_multipliers)
            new_total *= me.magnitude;
        _total = Mathf.Clamp(new_total, MIN, MAX);
    }

    public void Update()
    {
        if (_list_multipliers.Count > 0)
        {
            List<MultiplierElement> to_remove = new List<MultiplierElement>();
            foreach (MultiplierElement me in _list_multipliers)
            {
                if (Time.time > me.timestamp)
                    to_remove.Add(me);
            }
            foreach (MultiplierElement me in to_remove)
            {
                _list_multipliers.Remove(me);
            }
            UpdateTotal();
        }
    }
}
