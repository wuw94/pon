namespace UI
{
    using UnityEngine;

    public partial struct Container
    {
        public static Rect GetScaled(Rect container)
        {
            return GetScaled(container, Anchor.UpperLeft, Size.one, Offset.zero, Margin.zero);
        }

        public static Rect GetScaled(Rect container, Anchor anchor, Size dimension)
        {
            return GetScaled(container, anchor, dimension, Offset.zero, Margin.zero);
        }

        public static Rect GetScaled(Rect container, Anchor anchor, Size dimension, Margin margin)
        {
            return GetScaled(container, anchor, dimension, Offset.zero, margin);
        }

        public static Rect GetScaled(Rect container, Anchor anchor, Size dimension, Offset offset)
        {
            return GetScaled(container, anchor, dimension, offset, Margin.zero);
        }

        /// <summary>
        /// Scaled with parent container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="anchor"></param>
        /// <param name="dimension"></param>
        /// <param name="offset"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static Rect GetScaled(Rect container, Anchor anchor, Size dimension, Offset offset, Margin margin)
        {
            switch (anchor)
            {
                case Anchor.UpperLeft:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + (0),
                                        (container.height * offset.y) + (container.height * margin.top) + (0),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.UpperCenter:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + ((container.width - container.width * dimension.x) / 2),
                                        (container.height * offset.y) + (container.height * margin.top) + (0),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.UpperRight:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + (container.width - container.width * dimension.x),
                                        (container.height * offset.y) + (container.height * margin.top) + (0),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.MiddleLeft:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + (0),
                                        (container.height * offset.y) + (container.height * margin.top) + ((container.height - container.height * dimension.y) / 2),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.MiddleCenter:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + ((container.width - container.width * dimension.x) / 2),
                                        (container.height * offset.y) + (container.height * margin.top) + ((container.height - container.height * dimension.y) / 2),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.MiddleRight:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + (container.width - container.width * dimension.x),
                                        (container.height * offset.y) + (container.height * margin.top) + ((container.height - container.height * dimension.y) / 2),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.LowerLeft:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + (0),
                                        (container.height * offset.y) + (container.height * margin.top) + (container.height - container.height * dimension.y),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.LowerCenter:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + ((container.width - container.width * dimension.x) / 2),
                                        (container.height * offset.y) + (container.height * margin.top) + (container.height - container.height * dimension.y),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
                case Anchor.LowerRight:
                    return new Rect(
                                        (container.width * offset.x) + (container.width * margin.left) + (container.width - container.width * dimension.x),
                                        (container.height * offset.y) + (container.height * margin.top) + (container.height - container.height * dimension.y),
                                        (container.width * dimension.x) - (container.width * (margin.left + margin.right)),
                                        (container.height * dimension.y) - (container.height * (margin.top + margin.bottom))
                                    );
            }
            return Rect.zero;
        }
    }
}