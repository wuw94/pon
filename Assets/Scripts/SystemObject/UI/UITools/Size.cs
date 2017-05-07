namespace UI
{
    /// <summary>
    /// A UI element's size, i.e. its Width and Height
    /// </summary>
    public struct Size
    {
        public float x;
        public float y;

        public Size(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Size operator *(Size a, float b)
        {
            return new Size(a.x * b, a.y * b);
        }

        public static Size zero
        {
            get
            {
                return new Size(0, 0);
            }
        }
        public static Size one
        {
            get
            {
                return new Size(1, 1);
            }
        }
    }
}