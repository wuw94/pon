namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class HoverTextSystem : SystemBase
    {
        public UISystem uiSystem;
        private Queue<HoverText> hoverTexts = new Queue<HoverText>();
        public GUIStyle style = Resources.Load<Style>("Menu/PreGame/HoverText/style");
        
        public HoverTextSystem(UISystem uiSystem)
        {
            this.uiSystem = uiSystem;
        }

        public override void Update() { }

        public override void OnGUI()
        {
            while (hoverTexts.Count > 0)
                hoverTexts.Dequeue().OnGUI();
        }

        public void Enqueue(Rect mouseBounds, Rect textBounds, string text)
        {
            hoverTexts.Enqueue(new HoverText(this, mouseBounds, textBounds, text, style));
        }
    }
}