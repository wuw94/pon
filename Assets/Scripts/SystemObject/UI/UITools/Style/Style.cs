namespace UI
{
    public class Style : UnityEngine.ScriptableObject
    {
        public UnityEngine.GUIStyle style;

        public static implicit operator UnityEngine.GUIStyle(Style s)
        {
            return s.style;
        }
    }
}