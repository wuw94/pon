using UnityEngine;
using System.Collections.Generic;

public static class ContainerExtension
{
    /// <summary>
    /// Splits containers down to a range size, and returns all created containers as a list
    /// </summary>
    /// <param name="mine"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static List<ContainerXXX> RecursiveDivision(this ContainerXXX mine, IntRange range)
    {
        
        List<ContainerXXX> to_return = new List<ContainerXXX>();

        // Get random dimensions.
        int size = range.Random();
        Point container_dimension = new Point(size, Random.Range(size - 1, size + 1));
        for (int i = 0; i < range.max; i++)
        {
            if (container_dimension.x < range.min || container_dimension.y < range.min)
                return to_return;
            if (container_dimension.x > mine.dimension.x || container_dimension.y > mine.dimension.y) // If these dimensions are more than the rectangle
                container_dimension -= new Point(1, 1);
            else
                break;
        }

        Corner corner = CornerExtension.GetRandomCorner();

        ContainerSplitPacket csp = mine.QuadSplit(container_dimension, corner);
        to_return.Add(csp.result);
        foreach (ContainerXXX container in csp.container1.RecursiveDivision(range))
            to_return.Add(container);
        foreach (ContainerXXX container in csp.container2.RecursiveDivision(range))
            to_return.Add(container);

        return to_return;
    }



    private static ContainerSplitPacket QuadSplit(this ContainerXXX mine, Point dimension, Corner corner)
    {
        ContainerSplitPacket to_return = new ContainerSplitPacket();
        ContainerXXX b1 = new ContainerXXX(); //  [ b1  b2 ]
        ContainerXXX b2 = new ContainerXXX(); //  [ b1  b2 ]
        ContainerXXX b3 = new ContainerXXX();
        ContainerXXX b4 = new ContainerXXX();


        switch (corner)
        {
            case Corner.TopLeft:
                b1 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Left(Depth.Building) + dimension.x, mine.Top(Depth.Building), mine.Top(Depth.Building) - dimension.y);
                b2 = new ContainerXXX(mine.owner, mine.Left(Depth.Building) + dimension.x, mine.Right(Depth.Building), mine.Top(Depth.Building), mine.Top(Depth.Building) - dimension.y);
                b3 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Left(Depth.Building) + dimension.x, mine.Top(Depth.Building) - dimension.y, mine.Bottom(Depth.Building));
                b4 = new ContainerXXX(mine.owner, mine.Left(Depth.Building) + dimension.x, mine.Right(Depth.Building), mine.Top(Depth.Building) - dimension.y, mine.Bottom(Depth.Building));
                to_return.result = b1;
                break;
            case Corner.TopRight:
                b1 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Right(Depth.Building) - dimension.x, mine.Top(Depth.Building), mine.Top(Depth.Building) - dimension.y);
                b2 = new ContainerXXX(mine.owner, mine.Right(Depth.Building) - dimension.x, mine.Right(Depth.Building), mine.Top(Depth.Building), mine.Top(Depth.Building) - dimension.y);
                b3 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Right(Depth.Building) - dimension.x, mine.Top(Depth.Building) - dimension.y, mine.Bottom(Depth.Building));
                b4 = new ContainerXXX(mine.owner, mine.Right(Depth.Building) - dimension.x, mine.Right(Depth.Building), mine.Top(Depth.Building) - dimension.y, mine.Bottom(Depth.Building));
                to_return.result = b2;
                break;
            case Corner.BottomLeft:
                b1 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Left(Depth.Building) + dimension.x, mine.Top(Depth.Building), mine.Bottom(Depth.Building) + dimension.y);
                b2 = new ContainerXXX(mine.owner, mine.Left(Depth.Building) + dimension.x, mine.Right(Depth.Building), mine.Top(Depth.Building), mine.Bottom(Depth.Building) + dimension.y);
                b3 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Left(Depth.Building) + dimension.x, mine.Bottom(Depth.Building) + dimension.y, mine.Bottom(Depth.Building));
                b4 = new ContainerXXX(mine.owner, mine.Left(Depth.Building) + dimension.x, mine.Right(Depth.Building), mine.Bottom(Depth.Building) + dimension.y, mine.Bottom(Depth.Building));
                to_return.result = b3;
                break;
            case Corner.BottomRight:
                b1 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Right(Depth.Building) - dimension.x, mine.Top(Depth.Building), mine.Bottom(Depth.Building) + dimension.y);
                b2 = new ContainerXXX(mine.owner, mine.Right(Depth.Building) - dimension.x, mine.Right(Depth.Building), mine.Top(Depth.Building), mine.Bottom(Depth.Building) + dimension.y);
                b3 = new ContainerXXX(mine.owner, mine.Left(Depth.Building), mine.Right(Depth.Building) - dimension.x, mine.Bottom(Depth.Building) + dimension.y, mine.Bottom(Depth.Building));
                b4 = new ContainerXXX(mine.owner, mine.Right(Depth.Building) - dimension.x, mine.Right(Depth.Building), mine.Bottom(Depth.Building) + dimension.y, mine.Bottom(Depth.Building));
                to_return.result = b4;
                break;
        }

        if (dimension.x < dimension.y) // means it's taller than it is wide, so we do a vertical split
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    to_return.container1 = b3;
                    to_return.container2 = b2.Join(b4);
                    break;
                case Corner.TopRight:
                    to_return.container1 = b4;
                    to_return.container2 = b1.Join(b3);
                    break;
                case Corner.BottomLeft:
                    to_return.container1 = b1;
                    to_return.container2 = b2.Join(b4);
                    break;
                case Corner.BottomRight:
                    to_return.container1 = b2;
                    to_return.container2 = b1.Join(b3);
                    break;
            }
        }
        else // if its wider than it is tall, we do a horizontal split
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    to_return.container1 = b2;
                    to_return.container2 = b3.Join(b4);
                    break;
                case Corner.TopRight:
                    to_return.container1 = b1;
                    to_return.container2 = b3.Join(b4);
                    break;
                case Corner.BottomLeft:
                    to_return.container1 = b4;
                    to_return.container2 = b1.Join(b2);
                    break;
                case Corner.BottomRight:
                    to_return.container1 = b3;
                    to_return.container2 = b1.Join(b2);
                    break;
            }
        }

        return to_return;
    }
}

/// <summary>
/// A struct to hold all our information after splitting
/// </summary>
public struct ContainerSplitPacket
{
    /// <summary>
    /// The result container of our split.
    /// </summary>
    public ContainerXXX result;

    /// <summary>
    /// The smaller of the containers after the split.
    /// </summary>
    public ContainerXXX container1;

    /// <summary>
    /// The larger of the containers after ths split.
    /// </summary>
    public ContainerXXX container2;

    public ContainerSplitPacket(ContainerXXX result, ContainerXXX container1, ContainerXXX container2)
    {
        this.result = result;
        this.container1 = container1;
        this.container2 = container2;
    }
}