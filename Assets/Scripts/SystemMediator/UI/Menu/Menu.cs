namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    
    public class Menu : MonoBehaviour
    {
        protected MenuSystem menuSystem;

        protected virtual void Awake()
        {
            menuSystem = FindObjectOfType<MenuSystem>();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                OnKeyTab();
            if (Input.GetKeyDown(KeyCode.Escape))
                OnKeyEscape();
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }


        protected virtual void OnKeyTab()
        {
            if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() != null)
            {
                Selectable next;
                if (Input.GetKey(KeyCode.LeftShift))
                    next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                else
                    next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

                if (next != null)
                {
                    InputField inputfield = next.GetComponent<InputField>();
                    if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(EventSystem.current));  //if it's an input field, also set the text caret
                    EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
                }
                //else Debug.Log("next nagivation element not found");
            }
        }

        protected virtual void OnKeyEscape()
        {
            Back();
        }

        public virtual void Back() // This needs to be public to be called from inspector
        {
            menuSystem.Back();
        }
    }
}