namespace UI
{
    using UnityEngine;

    public class MenuSystem : MonoBehaviour
    {
        public UISystem uiSystem;
        public MenuStack menuStack = new MenuStack();
        public Menu current;

        public LoadingSystem loadingSystem;

        private void Awake()
        {
            if (menuStack.Count == 0)
                menuStack.Push(current);
        }
	
	    private void Update()
        {
            current = menuStack.current;
        }

        public void Push(Menu next)
        {
            if (menuStack.Count > 0)
                menuStack.current.gameObject.SetActive(false);
            menuStack.Push(next);
            if (!loadingSystem.isLoading)
                menuStack.current.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Go back in menus, which is popping off from the menu stack.
        /// </summary>
        public void Back()
        {
            if (menuStack.Count == 1)
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
            else
            {
                menuStack.current.gameObject.SetActive(false);
                menuStack.Pop();
                if (!loadingSystem.isLoading)
                    menuStack.current.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Go in menus until you hit a specific menu.
        /// </summary>
        /// <param name="menu"></param>
        public void BackUntil(Menu menu)
        {
            while (menuStack.current != menu)
                Back();
        }
    }
}