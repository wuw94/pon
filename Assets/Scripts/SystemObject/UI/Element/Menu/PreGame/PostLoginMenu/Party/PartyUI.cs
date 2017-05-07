namespace UI
{
    using UnityEngine;
    
    public class PartyUI : Element
    {
        private GUIStyle style = Resources.Load<Style>("Menu/PreGame/PostLogin/Party/style");
        private Database.Mediator.Party partyData;
        
        public PartyUI(MenuSystem menuSystem) : base(menuSystem.uiSystem)
        {
            partyData = uiSystem.systemObject.dataSystem.party;
        }

        public override void Update()
        {
            base.Update();

            style.fontSize = uiSystem.fontSize / 2;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (!uiSystem.systemObject.dataSystem.loggedIn)
                return;
            Rect r = Container.GetScaled(Container.screen, Anchor.UpperRight, new Size(0.4f, 0.08f), new Offset(-0.02f, 0.02f));
            GUI.BeginGroup(r);

            Size size_large = new Size(r.size.y / r.size.x, 1);
            Size size_medium = size_large * 0.75f;
            Size size_small = size_large * 0.5f;
            PartyDisplaySelf(r, size_large, size_medium, size_small);
            PartyDisplayOthers(r, size_large, size_medium, size_small);
            PartyButtonAdd(r, size_large, size_medium, size_small);

            GUI.EndGroup();
        }

        private void PartyDisplaySelf(Rect r, Size size_large, Size size_medium, Size size_small)
        {
            Size size = new Size(r.size.y / r.size.x, 1);
            Rect button_rect = Container.GetScaled(r, Anchor.UpperRight, size);
            if (GUI.Button(button_rect, partyData.IsLeader(uiSystem.systemObject.dataSystem.profile.userName) ? "L" : "", style))
            {
                uiSystem.systemObject.CoroutineStart(uiSystem.systemObject.dataSystem.query.Party.ChangeLeaderSingle(uiSystem.systemObject.dataSystem.profile.userName, uiSystem.systemObject.dataSystem.profile.userName));
            }
            PartyDisplayHoverText(uiSystem.systemObject.dataSystem.profile.userName, button_rect, r);
        }

        private void PartyDisplayOthers(Rect r, Size size_large, Size size_medium, Size size_small)
        {
            for (int i = 0; i < partyData.Count(); i++)
            {
                Rect button_rect = Container.GetScaled(r, Anchor.UpperRight, size_medium, new Offset(-size_large.x * 1.1f - i * size_medium.x * 1.1f, 0));
                if (GUI.Button(button_rect, partyData.IsLeader(partyData.Name(i)) ? "L" : "", style))
                {
                    if (partyData.IsLeader(uiSystem.systemObject.dataSystem.profile.userName))
                    {
                        uiSystem.systemObject.CoroutineStart(uiSystem.systemObject.dataSystem.query.Party.ChangeLeaderParty(partyData.Leader(), partyData.Name(i)));
                    }
                }
                PartyDisplayHoverText(partyData.Name(i), button_rect, r);
            }
        }

        private void PartyButtonAdd(Rect r, Size size_large, Size size_medium, Size size_small)
        {
            if (partyData.Count() < Database.Mediator.Party.MAX_SIZE)
            {
                Rect button_rect = Container.GetScaled(r, Anchor.UpperRight, size_small, new Offset(-size_large.x * 1.1f - (partyData.Count()) * size_medium.x * 1.1f, 0));
                if (GUI.Button(button_rect, "+", style))
                {
                    uiSystem.menuSystem.current = typeof(FriendsMenu);
                }

                PartyDisplayHoverText("Add party members", button_rect, r);
            }
        }

        private void PartyDisplayHoverText(string text, Rect button_rect, Rect r)
        {
            Rect mouseBounds = Container.GetAbsolute(r, button_rect);
            Vector2 textSize = uiSystem.hoverTextSystem.style.CalcSize(new GUIContent(text));
            Rect textBounds = new Rect(mouseBounds.position + new Vector2(mouseBounds.size.x / 2 - textSize.x / 2, button_rect.size.y + r.size.y * 0.05f), textSize);

            uiSystem.hoverTextSystem.Enqueue(mouseBounds, textBounds, text);
        }
    }

}