namespace UI
{
    using System;
    using System.Collections.Generic;

    public class NotificationSystem : SystemBase
    {
        private List<NotificationObject> notificationObjects = new List<NotificationObject>();

        public NotificationSystem()
        {
        }

        public override void Update() { }

        public override void OnGUI()
        {
            if (notificationObjects.Count > 0)
                notificationObjects[0].OnGUI();
        }

        public void Enqueue(NotificationObject notification)
        {
            notificationObjects.Add(notification);
        }

        public NotificationObject Dequeue()
        {
            NotificationObject to_return = notificationObjects[0];
            notificationObjects.RemoveAt(0);
            return to_return;
        }

        public void Remove(NotificationObject notification)
        {
            notificationObjects.Remove(notification);
        }

        public void Remove(Type type)
        {
            notificationObjects.RemoveAll(element => element.obj.GetType() == type);
        }
        
        public bool Contains(NotificationObject notification)
        {
            return notificationObjects.Contains(notification);
        }

        public bool Contains(Type type)
        {
            return notificationObjects.Exists(element => element.obj.GetType() == type);
        }
    }
}