using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MultiplicativeMultipliersList
{
    private const float MIN = 0;
    private const float MAX = 3;
    public struct MultiplierElement
    {
        public float multiplier;
        public float timestamp;
        public string source_name;
        public NetworkInstanceId source_owner;
        public MultiplierElement(float multiplier, float timestamp, string source_name, NetworkInstanceId source_owner)
        {
            this.multiplier = multiplier;
            this.timestamp = timestamp;
            this.source_name = source_name;
            this.source_owner = source_owner;
        }
    }

    private List<MultiplierElement> _list_multipliers = new List<MultiplierElement>();
    private float _total = 1;

    public MultiplicativeMultipliersList()
    {
    }

    public void Add(float multiplier, float duration, string source_name, NetworkInstanceId source_owner)
    {
        for (int i = 0; i < _list_multipliers.Count; i++)
        {
            if (_list_multipliers[i].source_name == source_name && _list_multipliers[i].source_owner == source_owner)
            {
                if (_list_multipliers[i].multiplier != multiplier)
                {
                    _list_multipliers[i] = new MultiplierElement(multiplier, Time.time + duration, source_name, source_owner);
                    UpdateTotal();
                }
                else
                {
                    _list_multipliers[i] = new MultiplierElement(multiplier, Time.time + duration, source_name, source_owner);
                }
                return;
            }
        }
        _list_multipliers.Add(new MultiplierElement(multiplier, Time.time + duration, source_name, source_owner));
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
            new_total *= me.multiplier;
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
