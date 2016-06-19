using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class IrregularRoom : Room
{
    /// <summary>
    /// An array of Vector2 specifying the grids occupied by this room.
    /// </summary>
    public Vector2[] Occupies;


    private void Awake()
    {
        base.Initialize();
    }

    public override bool Overlaps(RectRoom other)
    {
        foreach (Vector2 v in this.Occupies)
            if (v.x >= other.minX && v.x <= other.maxX && v.y >= other.minY && v.y <= other.maxY)
                return true;
        return false;
    }

    public override bool Overlaps(IrregularRoom other)
    {
        return this.Occupies.Intersect(other.Occupies).Count() != 0;
    }
}
