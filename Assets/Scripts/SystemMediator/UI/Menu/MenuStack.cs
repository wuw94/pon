namespace UI
{
    using System.Collections.Generic;

    public class MenuStack
    {
        private List<Menu> menus = new List<Menu>();
        public Menu current { get { return menus[menus.Count - 1]; } }
        public int Count { get { return menus.Count; } }

        public void Push(Menu newMenu) // Eventually change this to disallow duplicates in stack
        {
            menus.Add(newMenu);
        }

        public Menu Pop()
        {
            Menu m = menus[menus.Count - 1];
            menus.RemoveAt(menus.Count - 1);
            return m;
        }

        public void SetActiveAll(bool change_to)
        {
            foreach (Menu menu in menus)
                menu.gameObject.SetActive(change_to);
        }
    }
}