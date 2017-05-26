namespace Data.Database.MySQL
{
    public partial class Table
    {
        /// <summary>
        /// A MySQL Field, which is a column represented using an array of strings.
        /// </summary>
        public class Field
        {
            private string[] data;
            public int Length { get { return data.Length; } }

            internal Field()
            {
                data = new string[0];
            }

            internal Field(int rows)
            {
                data = new string[rows];
            }

            public string this[int index]
            {
                get
                {
                    return data[index];
                }
                set
                {
                    data[index] = value;
                }
            }

            public bool Contains(string value)
            {
                for (int i = 0; i < data.Length; i++)
                    if (data[i] == value)
                        return true;
                return false;
            }

            public override string ToString()
            {
                string to_return = "Field: " + Length + " rows.\n";
                for (int i = 0; i < Length; i++)
                {
                    to_return += data[i] + "\n";
                }
                return to_return;
            }
        }
    }
}