namespace UI
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;

    public class MenuSystem : SystemBase
    {
        public UISystem uiSystem;

        private Dictionary<Type, Menu> menus = new Dictionary<Type, Menu>();
        private Type _current = null;
        public Type current
        {
            get
            {
                return _current;
            }
            set
            {
                if (_current != null)
                    menus[_current].OnMenuExit();
                _current = value;
                menus[value].OnMenuEnter();
            }
        }

        public MenuSystem(UISystem uiSystem)
        {
            this.uiSystem = uiSystem;

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Menu)) && !t.IsAbstract))
                menus.Add(type, (Menu)Activator.CreateInstance(type, this));
            current = typeof(LoginMenu);
        }

        public override void Update()
        {
            menus[current].Update();
        }

        public override void OnGUI()
        {
            menus[current].OnGUI();
        }
    }
}