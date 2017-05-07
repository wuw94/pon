namespace UI
{
    using System.Collections;
    using UnityEngine;

    public class UISystem : SystemBase
    {
        public SystemObject systemObject;

        public NotificationSystem notificationSystem;
        public MenuSystem menuSystem;
        public HoverTextSystem hoverTextSystem;

        public int fontSize = 0;

        public UISystem(SystemObject systemObject)
        {
            this.systemObject = systemObject;
            
            notificationSystem = new NotificationSystem();
            menuSystem = new MenuSystem(this);
            hoverTextSystem = new HoverTextSystem(this);
        }

        public override void Update()
        {
            fontSize = Mathf.Min(40, Screen.height / 22);

            notificationSystem.Update();
            menuSystem.Update();
            hoverTextSystem.Update();
        }

        public override void OnGUI()
        {
            notificationSystem.OnGUI();
            menuSystem.OnGUI();
            hoverTextSystem.OnGUI();
        }

        /// <summary>
        /// Show where this group is.
        /// </summary>
        /// <param name="r"></param>
        public static void ShowContainer(Rect r)
        {
            GUI.Box(new Rect(Vector2.zero, r.size), "");
        }

        /// <summary>
        /// Returns true if mouse is over a rect
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool MouseOver(Rect r)
        {
            return r.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        }

        public Coroutine CoroutineStart(IEnumerator routine)
        {
            return systemObject.CoroutineStart(routine);
        }

        public void CoroutineStop(Coroutine coroutine)
        {
            systemObject.CoroutineStop(coroutine);
        }
    }
}