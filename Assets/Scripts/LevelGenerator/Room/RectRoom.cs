using UnityEngine;
using System.Collections;
using System;

public class RectRoom : Room
{
    public int minX { get { return (int)transform.position.x; } }
    public int maxX { get { return (int)transform.position.x + width - 1; } }
    public int minY { get { return (int)transform.position.y; } }
    public int maxY { get { return (int)transform.position.y + height - 1; } }

    private void Awake()
    {
        base.Initialize();
    }

    public override bool Overlaps(RectRoom other)
    {
        return this.minX <= other.maxX && other.minX <= this.maxX && this.minY <= other.maxY && other.minY <= this.maxY;
    }

    public override bool Overlaps(IrregularRoom other)
    {
        return other.Overlaps(this);
    }
}
