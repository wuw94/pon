namespace UI
{
    using UnityEngine;

    public partial struct Container
    {
        public static Rect Get(Rect container)
        {
            return Get(container, Anchor.UpperLeft, new Size(container.width, container.height), Offset.zero, Margin.zero);
        }

        public static Rect Get(Rect container, Anchor anchor, Size dimension)
        {
            return Get(container, anchor, dimension, Offset.zero, Margin.zero);
        }

        public static Rect Get(Rect container, Anchor anchor, Size dimension, Margin margin)
        {
            return Get(container, anchor, dimension, Offset.zero, margin);
        }

        public static Rect Get(Rect container, Anchor anchor, Size dimension, Offset offset)
        {
            return Get(container, anchor, dimension, offset, Margin.zero);
        }


        // 0,0 is top left. +X = right, +Y = down
        public static Rect Get(Rect container, Anchor anchor, Size dimension, Offset offset, Margin margin)
        {
            switch (anchor)
            {
                case Anchor.UpperLeft:
                    return new Rect(
                                                    (offset.x) + (margin.left) + (0),
                                                    (offset.y) + (margin.top) + (0),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.UpperCenter:
                    return new Rect(
                                                    (offset.x) + (margin.left) + ((container.width - dimension.x) / 2),
                                                    (offset.y) + (margin.top) + (0),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.UpperRight:
                    return new Rect(
                                                    (offset.x) + (margin.left) + (container.width - dimension.x),
                                                    (offset.y) + (margin.top) + (0),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.MiddleLeft:
                    return new Rect(
                                                    (offset.x) + (margin.left) + (0),
                                                    (offset.y) + (margin.top) + ((container.height - dimension.y) / 2),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.MiddleCenter:
                    return new Rect(
                                                    (offset.x) + (margin.left) + ((container.width - dimension.x) / 2),
                                                    (offset.y) + (margin.top) + ((container.height - dimension.y) / 2),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.MiddleRight:
                    return new Rect(
                                                    (offset.x) + (margin.left) + (container.width - dimension.x),
                                                    (offset.y) + (margin.top) + ((container.height - dimension.y) / 2),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.LowerLeft:
                    return new Rect(
                                                    (offset.x) + (margin.left) + (0),
                                                    (offset.y) + (margin.top) + (container.height - dimension.y),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.LowerCenter:
                    return new Rect(
                                                    (offset.x) + (margin.left) + ((container.width - dimension.x) / 2),
                                                    (offset.y) + (margin.top) + (container.height - dimension.y),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
                case Anchor.LowerRight:
                    return new Rect(
                                                    (offset.x) + (margin.left) + (container.width - dimension.x),
                                                    (offset.y) + (margin.top) + (container.height - dimension.y),
                                                    (dimension.x) - (margin.left + margin.right),
                                                    (dimension.y) - (margin.top + margin.bottom)
                                                );
            }
            return Rect.zero;
        }
    }
}