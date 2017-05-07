namespace Database.MySQL
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// For pasting MySQL output to a table using the WWW class and a PHP url.
    /// </summary>
    public partial class Query
    {
        private const string SECRET_KEY = "485AD1953719843C46CE785ECBF74";
        private const string SERVER_URL = "http://45.50.73.43/PHP/pon/call.php";
        private const char ROW_DELIMITER = '\n';
        private const char FIELD_DELIMITER = ' ';

        public Query()
        {
        }

        private IEnumerator Run(string function, params string[] vars)
        {
            yield return Run(new Table(), function, vars);
        }

        private IEnumerator Run(Table output, string function, params string[] vars)
        {
            if (vars.Length % 2 != 0)
                Debug.LogError("Error running MySQL Reader: Odd-number field-value pairs");
            
            string hash = "";
            for (int i = 0; i < vars.Length; i += 2)
                hash += vars[i + 1];
            hash = Validation.Md5Sum(hash + SECRET_KEY);

            string url = "";
            url = SERVER_URL + "?function=" + function;
            for (int i = 0; i < vars.Length; i += 2)
                url += "&" + vars[i] + "=" + WWW.EscapeURL(vars[i + 1]);
            url += "&hash=" + hash;

            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            WWW result = new WWW(url);
            yield return result; // After this line, we've finished downloading our data

            //Debug.Log(s.ElapsedMilliseconds);


            // Process result

            Table table = new Table(); // Create table as output. Default success=false
            if (result.error == null) // Download succeeded
            {
                string[] line = result.text.Split('\n');
                if (line[0] == "MySQL: Success") // MySQL succeeded
                {
                    if (line[1] != "Query: Failed") // Query succeeded
                    {
                        //Debug.Log("MySQL Succeeded\nURL:\n  " + url + "\n\nResult:\n" + result.text);
                        output.success = true;
                        if (line[1] != "Query:  rows") // A pull, not push statement
                        {
                            table = new Table(int.Parse(line[1].Split(' ')[1])); // set rows for table
                            for (int i = 0; i < table.Length; i++)
                            {
                                string[] rows = line[i + 3].Split(' '); // segment is one row, field=value field=value

                                for (int j = 0; j < rows.Length; j++)
                                {
                                    string[] pair = rows[j].Split('='); // pair[0] holds field name, pair[1] holds value
                                    if (!table.HasField(pair[0]))
                                        table.AddField(pair[0]);
                                    table[pair[0]][i] = pair[1];
                                }
                            }
                        }
                        output.Set(table);
                    }
                    else // Query Failed
                    {
                        Debug.LogError("Query Failed\nURL:\n" + url + "\n\nResult:\n" + result.text);
                    }
                }
                else // MySQL Failed
                {
                    Debug.LogError("MySQL Failed\nURL:\n  " + url + "\n\nResult:\n" + result.text);
                }
            }
            else // Download failed
            {
                Debug.LogError("WWW Download failed:\n" + result.error);
            }

            yield return null;
        }

        

    }
}