namespace Database.MySQL
{
    using System.Collections;

    public interface IAccountMethods
    {
        IEnumerator SetActive(string name);
        IEnumerator Search(Table output, string name);
        IEnumerator List(Table output, string name);
        IEnumerator List(Table output, string name, string password);
        IEnumerator Create(string name, string password, string email);
    }

    public partial class Query : IAccountMethods
    {
        public IAccountMethods Account { get { return this as IAccountMethods; } }

        /// <summary>
        /// Set this account as active. This is so people can see who is online or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerator IAccountMethods.SetActive(string name)
        {
            yield return Run("AccountSetActive(name)", "name", name);
        }

        /// <summary>
        /// List accounts that start with this string.
        /// Accessors ["name"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerator IAccountMethods.Search(Table output, string name)
        {
            yield return Run(output, "AccountSearch(name)", "name", name);
        }

        /// <summary>
        /// List accounts that match this name.
        /// Accessors ["name"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerator IAccountMethods.List(Table output, string name)
        {
            yield return Run(output, "AccountList(name)", "name", name);
        }

        /// <summary>
        /// List accounts that match this name and password.
        /// Accessors ["name"]
        /// </summary>
        /// <param name="output"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        IEnumerator IAccountMethods.List(Table output, string name, string password)
        {
            yield return Run(output, "AccountList(name,password)", "name", name, "password", password);
        }

        /// <summary>
        /// Create an account with this name, password, and email.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        IEnumerator IAccountMethods.Create(string name, string password, string email)
        {
            yield return Run("AccountCreate(name,password,email)", "name", name, "password", password, "email", email);
        }
    }
}