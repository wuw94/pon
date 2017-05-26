namespace UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ListGamesMenu : Menu
    {
        public Transform buttonParent;
        public GameObject button;
        private Queue<Button> buttons = new Queue<Button>();
        public RectTransform content;
        public Menu ConfigureGameMenu;

        public Data.Database.Mediator.Match.MatchInfo matchInfo = Data.Database.Mediator.Match.MatchInfo.Invalid;

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        public void Refresh()
        {
            matchInfo = Data.Database.Mediator.Match.MatchInfo.Invalid; // Invalidate our selected match
            while (buttons.Count > 0) // Clear all existing buttons.
            {
                Button button = buttons.Dequeue();
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

            Data.Database.Mediator.Match match = menuSystem.uiSystem.systemMediator.dataSystem.databaseSystem.match;
            content.sizeDelta = new Vector2(0, button.GetComponent<RectTransform>().rect.height * match.Count()); // Resize contents
            for (int i = 0; i < match.Count(); i++)
            {
                Button matchButton = Instantiate(button, buttonParent).GetComponent<Button>();
                buttons.Enqueue(matchButton);
                matchButton.GetComponentInChildren<Text>().text = match[i].name + "'s Game";
                int x = i;
                matchButton.onClick.AddListener(() => ButtonClicked(x));
            }
        }

        private void ButtonClicked(int index)
        {
            Data.Database.Mediator.Match match = menuSystem.uiSystem.systemMediator.dataSystem.databaseSystem.match;
            matchInfo = match[index];
        }

        public void Join()
        {
            Data.Network.NetworkSystem networkSystem = menuSystem.uiSystem.systemMediator.dataSystem.networkSystem;
            if (matchInfo == Data.Database.Mediator.Match.MatchInfo.Invalid)
            {
                Debug.LogError("Invalid Match");
                return;
            }
            networkSystem.EnsureJoinedMatchExists(matchInfo);
            networkSystem.Join(matchInfo);
            menuSystem.Push(ConfigureGameMenu);
        }

        public override void Back()
        {
            base.Back();
            menuSystem.uiSystem.systemMediator.dataSystem.networkSystem.StopModule();
        }
    }
}