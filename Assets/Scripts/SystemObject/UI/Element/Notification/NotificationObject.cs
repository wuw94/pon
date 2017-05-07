namespace UI
{
    public class NotificationObject
    {
        public object obj; // Whether or not two notifications are the same depends on whether this object is the same.
        public NotificationUIDelegate OnGUI; // Delegate to call when displaying the notification on the GUI loop.

        public NotificationObject(object obj, NotificationUIDelegate OnGUI)
        {
            this.obj = obj;
            this.OnGUI = OnGUI;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return this.obj.Equals((obj as NotificationObject).obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}