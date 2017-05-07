using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A directional. Positions are relative to room. Access through 'owner'
/// </summary>
[System.Serializable]
public class Directional : ContainerXXX
{
    public Direction direction;

    public Directional()
        : base() { }

    public Directional(ContainerXXX owner, Point relative_position, Direction direction)
        : this(new ContainerXXX(owner, relative_position, new Point(1, 1)), direction) { }

    public Directional(ContainerXXX container, Direction direction)
        : base(container)
    {
        this.direction = direction;
    }
    
    public static bool operator ==(Directional directional1, Directional directional2)
    {
        return directional1.Position(Depth.World) == directional2.Position(Depth.World) && directional1.direction == directional2.direction;
    }

    public static bool operator !=(Directional directional1, Directional directional2)
    {
        return directional1.Position(Depth.World) != directional2.Position(Depth.World) || directional1.direction != directional2.direction;
    }
    
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        string to_return = "Directional - " + "Pos" + this.position_at_current_depth + " Dim" + this.dimension + " Depth(" + this.depth + ")" + " Direction(" + this.direction + ")";
        to_return += "\n Depth " + this.depth;
        to_return += "\n Depth at World: Pos" + this.Position(Depth.World);
        return to_return;
    }
}

[System.Serializable]
public struct DirectionalPair
{
    public Directional first;
    public Directional second;

    public DirectionalPair(Directional first, Directional second)
    {
        this.first = first;
        this.second = second;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(DirectionalPair wp1, DirectionalPair wp2)
    {
        return (wp1.first == wp2.first && wp1.second == wp2.second) || (wp1.first == wp2.second && wp1.second == wp2.first);
    }

    public static bool operator !=(DirectionalPair wp1, DirectionalPair wp2)
    {
        return wp1.first != wp2.first || wp1.first != wp2.second || wp1.second != wp2.first || wp1.second != wp2.second;
    }

    public override string ToString()
    {
        string to_return = "DirectionalPair - ";
        to_return += "First\n" + first;
        to_return += "\nSecond\n" + second;
        return to_return;
    }
}

public static class DirectionalExtension
{
    /// <summary>
    /// By the logic of this game, inclusion objects are only half of a whole directional. Use this to get a directional's reciprocal.
    /// </summary>
    /// <param name="directional"></param>
    /// <returns></returns>
    public static Directional OtherSide(this Directional directional)
    {
        Directional to_return = new Directional(directional, directional.direction);
        to_return.position_at_current_depth += directional.direction.Offset();
        to_return.direction = directional.direction.Opposite();
        return to_return;
    }
}