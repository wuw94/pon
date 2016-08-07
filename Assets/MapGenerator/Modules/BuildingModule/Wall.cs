using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A wall. Positions are relative to room. Access through 'owner'
/// </summary>
public class Wall : Container
{
    public Direction direction;

    public Wall()
        : base() { }

    public Wall(Container owner, Point relative_position, Direction direction)
        : this(new Container(owner, relative_position, new Point(1, 1)), direction) { }

    public Wall(Container container, Direction direction)
        : base(container)
    {
        this.direction = direction;
    }
    
    public static bool operator ==(Wall wall1, Wall wall2)
    {
        return wall1.Position(Depth.Building) == wall2.Position(Depth.Building) && wall1.direction == wall2.direction;
    }

    public static bool operator !=(Wall wall1, Wall wall2)
    {
        return wall1.Position(Depth.Building) != wall2.Position(Depth.Building) || wall1.direction != wall2.direction;
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
        string to_return = "Wall - " + "Pos" + this.position_at_current_depth + " Dim" + this.dimension + " Depth(" + this.depth + ")" + " Direction(" + this.direction + ")";
        to_return += "\n Depth " + this.depth;
        to_return += "\n Depth at Building: Pos" + this.Position(Depth.Building);
        return to_return;
    }
}

public struct WallPair
{
    public Wall first;
    public Wall second;

    public WallPair(Wall first, Wall second)
    {
        this.first = first;
        this.second = second;
    }

    public override string ToString()
    {
        string to_return = "WallPair - ";
        to_return += "First\n" + first;
        to_return += "\nSecond\n" + second;
        return to_return;
    }
}

public static class WallExtension
{
    /// <summary>
    /// By the logic of this game, wall objects are only half of a whole wall. Use this to get a wall's reciprocal.
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    public static Wall OtherSide(this Wall wall)
    {
        Wall to_return = new Wall(wall, wall.direction);
        to_return.position_at_current_depth += wall.direction.Offset();
        to_return.direction = wall.direction.Opposite();
        return to_return;
    }
    
    

    

}