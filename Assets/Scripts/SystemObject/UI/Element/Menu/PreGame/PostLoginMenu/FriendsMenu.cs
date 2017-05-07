namespace UI
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class FriendsMenu : PostLoginMenu
    {
        public GUIStyle StyleCategoryActive = Resources.Load<Style>("Menu/PreGame/PostLogin/FriendsMenu/Category/Active/style");
        public GUIStyle StyleCategoryInactive = Resources.Load<Style>("Menu/PreGame/PostLogin/FriendsMenu/Category/Inactive/style");

        public GUIStyle StylePersonOnline = Resources.Load<Style>("Menu/PreGame/PostLogin/FriendsMenu/Person/Online/style");
        public GUIStyle StylePersonOffline = Resources.Load<Style>("Menu/PreGame/PostLogin/FriendsMenu/Person/Offline/style");

        public GUIStyle StylePersonAdd = Resources.Load<Style>("Menu/PreGame/PostLogin/FriendsMenu/Person/Add/style");

        // Left Side
        private enum LeftGroupSelection { Search, All, Online, Incoming_Requests, Pending_Requests }
        private string[] left_group_name = new string[] { "Search", "All", "Online", "Incoming Requests", "Pending Requests" };
        private LeftGroupSelection left_group_current;

        // Search
        private const string FOCUS_TEXTBOX_SEARCH = "Search";
        private string name_search = "";
        private bool name_search_changed;


        // Right Side
        private int num_elements;
        private Vector2 scroll_value = Vector2.zero;


        private Vector2 friend_list_group_dim;

        private Database.DataSystem dataSystem;
        private Database.MySQL.Query query;
        private Database.MySQL.Table searchTable = new Database.MySQL.Table();

        private Database.Mediator.Friend friendData;
        private Database.Mediator.FriendRequest friendRequestData;

        public FriendsMenu(MenuSystem menu_system) : base(menu_system)
        {
            dataSystem = uiSystem.systemObject.dataSystem;
            query = uiSystem.systemObject.dataSystem.query;
            friendData = dataSystem.friend;
            friendRequestData = dataSystem.friendRequest;
            title = "Friends";
        }

        public override void OnMenuEnter()
        {
            base.OnMenuEnter();
            name_search = "";
            left_group_current = LeftGroupSelection.All;
        }


        public override void Update()
        {
            base.Update();

            StyleCategoryActive.fontSize = StyleCategoryInactive.fontSize = uiSystem.fontSize * 8 / 10;
            StylePersonOnline.fontSize = StylePersonOffline.fontSize = uiSystem.fontSize;
            StylePersonAdd.fontSize = uiSystem.fontSize * 3 / 2;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            LeftGroup();
            RightGroup();

            //FriendListGroup();
            BackButton("Escape", "Back to Home");
        }


        // Left Group
        private void LeftGroup()
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.UpperLeft, new Size(0.23f, 0.5f), new Offset(0.02f, 0.15f));
            GUI.BeginGroup(r);
            UISystem.ShowContainer(r);
            LeftSearch(r);
            LeftButtons(r);
            GUI.EndGroup();
        }

    

        private void LeftSearch(Rect r)
        {
            if (GUI.GetNameOfFocusedControl() == FOCUS_TEXTBOX_SEARCH)
                left_group_current = LeftGroupSelection.Search;
            GUI.SetNextControlName(FOCUS_TEXTBOX_SEARCH);
            string name_search_new = GUI.TextField(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.8f, 0.1f), new Offset(0, 0)), name_search, 15, StyleSearchTextField);

            if (name_search_new != name_search)
            {
                if (name_search_new == "")
                    searchTable.Clear();
                else
                    dataSystem.profile.Search(searchTable, name_search_new);
            }
            name_search = name_search_new;

            if (GUI.GetNameOfFocusedControl() != FOCUS_TEXTBOX_SEARCH && name_search == string.Empty)
            {
                GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.8f, 0.1f), new Offset(0, 0)), "Search Name...");
            }
        }

        private void LeftButtons(Rect r)
        {
            int[] a = new int[] { 0, friendData.online.Length + friendData.offline.Length, friendData.online.Length, friendRequestData.receiver.Length, friendRequestData.requester.Length };
            for (int i = 1; i < Enum.GetValues(typeof(LeftGroupSelection)).Length; i++)
            {
                string button_text = left_group_name[i] + ((a[i] != 0) ? " (" + a[i] + ")" : "");
                if (GUI.Button(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.9f, 0.1f), new Offset(0, 0.2f + 0.1f * i)), button_text, (left_group_current == (LeftGroupSelection)i) ? StyleCategoryActive : StyleCategoryInactive))
                {
                    name_search = "";
                    GUI.FocusControl("");
                    left_group_current = (LeftGroupSelection)i;
                }
            }
        }







        // Right Group
        private void RightGroup()
        {
            Rect r = Container.GetScaled(Container.screen, Anchor.UpperRight, new Size(0.7f, 0.77f), new Offset(-0.02f, 0.15f));
            scroll_value = GUI.BeginScrollView(r, scroll_value, new Rect(0, 0, r.width - 20, r.height * 0.1f * num_elements));
            RightListNames(r);
            GUI.EndScrollView();
        }

        private void RightListNames(Rect r)
        {
            switch (left_group_current)
            {
                case LeftGroupSelection.Search: // Search
                    num_elements = searchTable.Length;
                    for (int i = 0; i < num_elements; i++)
                    {
                        GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.7f, 0.1f), new Offset(0, i * 0.1f)), searchTable["name"][i], StylePersonOnline);
                        if (!friendRequestData.requester.Contains(searchTable["name"][i]) && !friendData.online.Contains(searchTable["name"][i]) && !friendData.offline.Contains(searchTable["name"][i]))
                        {
                            if (GUI.Button(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.1f, 0.1f), new Offset(0.7f, i * 0.1f)), "+", StylePersonAdd))
                            {
                                if (friendRequestData.receiver.Contains(searchTable["name"][i]))
                                    uiSystem.systemObject.CoroutineStart(query.FriendRequest.Accept(dataSystem.profile.userName, searchTable["name"][i]));
                                else
                                    uiSystem.systemObject.CoroutineStart(query.FriendRequest.Make(dataSystem.profile.userName, searchTable["name"][i]));
                            }
                        }
                    }
                    break;




                case LeftGroupSelection.All: // All
                    num_elements = friendData.online.Length + friendData.offline.Length;
                    for (int i = 0; i < friendData.online.Length; i++)
                    {
                        GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.7f, 0.1f), new Offset(0, i * 0.1f)), friendData.online[i], StylePersonOnline);
                        if (GUI.Button(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.1f, 0.1f), new Offset(0.7f, i * 0.1f)), "-", StylePersonOffline))
                        {
                            uiSystem.systemObject.CoroutineStart(query.Friend.Delete(dataSystem.profile.userName, friendData.online[i]));
                        }
                    }
                    for (int i = 0; i < friendData.offline.Length; i++)
                    {
                        GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.7f, 0.1f), new Offset(0, (i + 1 + friendData.online.Length) * 0.1f)), friendData.offline[i], StylePersonOnline);
                        if (GUI.Button(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.1f, 0.1f), new Offset(0.7f, (i + 1 + friendData.online.Length) * 0.1f)), "-", StylePersonAdd))
                        {
                            uiSystem.systemObject.CoroutineStart(query.Friend.Delete(dataSystem.profile.userName, friendData.offline[i]));
                        }
                    }
                    break;





                case LeftGroupSelection.Online: // Online
                    num_elements = friendData.online.Length;
                    for (int i = 0; i < num_elements; i++)
                    {
                        GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.7f, 0.1f), new Offset(0, i * 0.1f)), friendData.online[i], StylePersonOnline);
                        if (GUI.Button(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.1f, 0.1f), new Offset(0.7f, i * 0.1f)), "+", StylePersonAdd))
                        {
                            uiSystem.systemObject.CoroutineStart(query.PartyInvite.Make(dataSystem.profile.userName, friendData.online[i]));
                        }
                    }
                    break;





                case LeftGroupSelection.Incoming_Requests: // Incoming Requests
                    num_elements = friendRequestData.receiver.Length;
                    for (int i = 0; i < num_elements; i++)
                    {
                        GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.7f, 0.1f), new Offset(0, i * 0.1f)), friendRequestData.receiver[i], StylePersonOnline);
                        if (GUI.Button(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.1f, 0.1f), new Offset(0.7f, i * 0.1f)), "+", StylePersonAdd))
                        {
                            uiSystem.systemObject.CoroutineStart(query.FriendRequest.Accept(dataSystem.profile.userName, friendRequestData.receiver[i]));
                        }
                    }
                    break;





                case LeftGroupSelection.Pending_Requests: // Pending Requests
                    num_elements = friendRequestData.requester.Length;
                    for (int i = 0; i < num_elements; i++)
                    {
                        GUI.Label(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.7f, 0.1f), new Offset(0, i * 0.1f)), friendRequestData.requester[i], StylePersonOnline);
                        if (GUI.Button(Container.GetScaled(r, Anchor.UpperLeft, new Size(0.1f, 0.1f), new Offset(0.7f, i * 0.1f)), "-", StylePersonAdd))
                        {
                            uiSystem.systemObject.CoroutineStart(query.FriendRequest.Cancel(dataSystem.profile.userName, friendRequestData.requester[i]));
                        }
                    }
                    break;



            }
        }
    

        


        protected override void OnKeyEscape()
        {
            base.OnKeyEscape();
            Back();
        }

        protected override void Back()
        {
            base.Back();
            menuSystem.current = typeof(StandbyMenu);
        }
    }
}