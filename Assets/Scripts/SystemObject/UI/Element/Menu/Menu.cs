namespace UI
{
    using UnityEngine;

    public abstract class Menu : Element
    {
        protected MenuSystem menuSystem;

        /// <summary>
        /// Focus to set when we enter this menu.
        /// </summary>
        protected string focus = "";
        private bool _first_frame_passed;
        private bool _first_focus_set;

        public Menu(MenuSystem menuSystem) : base(menuSystem.uiSystem)
        {
            this.menuSystem = menuSystem;
        }

        /// <summary>
        /// What appears when we want to run the GUI of this menu class.
        /// </summary>
        public override void OnGUI()
        {
            base.OnGUI();

            if (_first_frame_passed)
            {
                if (!_first_focus_set)
                {
                    _first_focus_set = true;
                    GUI.FocusControl(focus);
                }
            }
            else
            {
                _first_frame_passed = true;
            }


            if (Event.current.isKey)
            {
                if (Event.current.keyCode == KeyCode.Return)
                    OnKeyEnter();
                if (Event.current.keyCode == KeyCode.Escape)
                    OnKeyEscape();
            }
        }


        /// <summary>
        /// Called right when we enter this menu.
        /// </summary>
        public virtual void OnMenuEnter()
        {
            _first_frame_passed = false;
            _first_focus_set = false;
        }

        /// <summary>
        /// Called right when we exit this menu (like when we move to another menu).
        /// </summary>
        public virtual void OnMenuExit() { }

        /// <summary>
        /// What happens when we press ESCAPE when we're using this menu class.
        /// </summary>
        protected virtual void OnKeyEscape() { }

        /// <summary>
        /// What happens when we press ENTER (Return) when we're using this menu class.
        /// </summary>
        protected virtual void OnKeyEnter() { }

        /// <summary>
        /// Go back. Specify where to go back to inside the method.
        /// </summary>
        protected virtual void Back() { }
    }
}