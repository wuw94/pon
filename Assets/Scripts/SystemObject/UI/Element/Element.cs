namespace UI
{
    public abstract class Element
    {
        public UISystem uiSystem;

        public Element(UISystem uiSystem)
        {
            this.uiSystem = uiSystem;
        }

        public virtual void Update() { }
        public virtual void OnGUI() { }
    }
}