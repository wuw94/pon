namespace Database.Mediator
{
    using System.Collections;
    using MySQL;

    public class Profile : DataMediator
    {
        public string userName = "";

        public Profile(DataSystem dataSystem) : base(dataSystem)
        {
            wait = 1;
        }

        protected override IEnumerator UpdateTables()
        {
            base.UpdateTables();

            if (dataSystem.loggedIn)
            {
                yield return query.Account.SetActive(userName); // Continuously ping the database server to let it know that I'm online.
            }
        }

        public void Search(Table output, string name)
        {
            CoroutineStart(query.Account.Search(output, name)); // access with "name"
        }
    }
}