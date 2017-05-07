namespace Database.Mediator
{
    using UnityEngine;
    using System.Collections;

    public class DataMediator
    {
        protected DataSystem dataSystem;
        protected MySQL.Query query;
        private Coroutine updaterCoroutine;
        protected float wait = 0.0f;

        public DataMediator(DataSystem dataSystem)
        {
            this.dataSystem = dataSystem;
            this.query = dataSystem.query;
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
            return dataSystem.CoroutineStart(routine);
        }

        public void CoroutineStop(Coroutine coroutine)
        {
            dataSystem.CoroutineStop(coroutine);
        }
    }
}