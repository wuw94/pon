namespace Data.Database.Mediator
{
    using UnityEngine;
    using System.Collections;

    public class DataMediator
    {
        protected DatabaseSystem databaseSystem;
        protected MySQL.Query query;
        private Coroutine updaterCoroutine;
        protected float wait = 0.0f;

        public DataMediator(DatabaseSystem databaseSystem)
        {
            this.databaseSystem = databaseSystem;
            this.query = databaseSystem.query;
            UpdaterStart();
        }

        protected void UpdaterStart()
        {
            updaterCoroutine = CoroutineStart(Updater());
        }

        protected void UpdaterStop()
        {
            CoroutineStop(updaterCoroutine);
        }

        private IEnumerator Updater()
        {
            while (true)
                yield return UpdateTables();
        }

        protected virtual IEnumerator UpdateTables()
        {
            yield return new WaitForSeconds(wait);
        }

        public Coroutine CoroutineStart(IEnumerator routine)
        {
            return databaseSystem.CoroutineStart(routine);
        }

        public void CoroutineStop(Coroutine coroutine)
        {
            databaseSystem.CoroutineStop(coroutine);
        }
    }
}