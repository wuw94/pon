namespace Data
{
    using System.Text.RegularExpressions;
    using System.Security.Cryptography;

    /// <summary>
    /// If you want to find out if something is valid, do it here.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Return true if this name is valid for the game
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidName(string s)
        {
            if (s == "")
                return false;
            return new Regex("^[a-zA-Z0-9]*$").IsMatch(s);
        }

        /// <summary>
        /// Return true if a string is in the form of an email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion) && Regex.Replace(email, expresion, string.Empty).Length == 0)
                return true;
            return false;
        }

        /// <summary>
        /// Md5Sum encryption
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <returns></returns>
        public static string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }
    }
}