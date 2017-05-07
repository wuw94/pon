namespace UI
{
    using UnityEngine;

    public partial struct Container
    {
        public static Rect screen
        {
            get
            {
                return new Rect(0, 0, Screen.width, Screen.height);
            }
        }

        /// <summary>
        /// Get the rectangle in absolute terms, which is according to the screen.
        /// Our parameters are an array of all the necessary containers to get to the lowest level rectangle.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Rect GetAbsolute(params Rect[] args)
        {
            Vector2 position = Vector2.zero;
            for (int i = 0; i < args.Length; i++)
                position += args[i].position;
            Vector2 size = args[args.Length - 1].size;
            return new Rect(position, size);
        }

        
    }
}