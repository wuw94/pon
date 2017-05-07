namespace UI
{
    /// <summary>
    /// A UI element's offset.
    /// (+X moves right, +Y moves down)
    /// </summary>
    public struct Offset
    {
        public float x;
        public float y;

        public Offset(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Offset zero
        {
            get
            {
                return new Offset(0, 0);
            }
        }
    }
}