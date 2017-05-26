namespace UI
{
    using UnityEngine;

    /// <summary>
    /// LoadingSystem halts menu display to display loading.
    /// </summary>
    public class LoadingSystem : MonoBehaviour
    {
        public MenuSystem menuSystem;
        public GameObject LoadingPanel;
        public bool isLoading = false;

        private void Update()
        {
            if (isLoading && menuSystem.uiSystem.systemMediator.dataSystem.networkSystem.Ready())
                RelinquishControl();
            else if (!isLoading && !menuSystem.uiSystem.systemMediator.dataSystem.networkSystem.Ready())
                SiezeControl();
        }

        private void SiezeControl()
        {
            isLoading = true;
            menuSystem.menuStack.SetActiveAll(false);
            LoadingPanel.SetActive(true);
        }

        private void RelinquishControl()
        {
            isLoading = false;
            LoadingPanel.SetActive(false);
            menuSystem.menuStack.current.gameObject.SetActive(true);
        }
    }
}