namespace UI
{
    /// <summary>
    /// A UI element's margins. Stored as top, right, bottom, left.
    /// </summary>
    public struct Margin
    {
        public float top;
        public float right;
        public float bottom;
        public float left;

        public Margin(float top, float right, float bottom, float left)
        {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
        }

        public static Margin zero
        {
            get
            {
                return new Margin(0, 0, 0, 0);
            }
        }
    }
}