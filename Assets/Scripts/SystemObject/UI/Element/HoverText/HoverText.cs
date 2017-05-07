namespace UI
{
    using UnityEngine;

    public class HoverText : Element
    {
        private Rect mouseBounds;
        private Rect textBounds;
        private string text;
        private GUIStyle style;

        public HoverText(HoverTextSystem hoverTextSystem, Rect mouseBounds, Rect textBounds, string text, GUIStyle style) : base(hoverTextSystem.uiSystem)
        {
            this.mouseBounds = mouseBounds;
            this.textBounds = textBounds;
            this.text = text;
            this.style = style;
        }

        public override void Update()
        {
            base.Update();

            style.fontSize = uiSystem.fontSize / 3;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (MouseOver(mouseBounds))
                GUI.Label(textBounds, text, style);
        }

        private bool MouseOver(Rect r)
        {
            return r.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        }
    }
}