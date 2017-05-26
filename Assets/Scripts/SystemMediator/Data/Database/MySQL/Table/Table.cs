namespace Data.Database.MySQL
{
    using System.Collections.Generic;

    /// <summary>
    /// A MySQL table, which is a dictionary of Fields (which are string arrays).
    /// Access with table["field name"][row#]
    /// </summary>
    public partial class Table
    {
        public bool success = false;

        public int Length;
        Dictionary<string, Field> table;

        public Table()
        {
            Length = 0;
            table = new Dictionary<string, Field>();
        }

        internal Table(int rows)
        {
            Length = rows;
            table = new Dictionary<string, Field>();
        }

        internal void Set(Table other)
        {
            Length = other.Length;
            table = other.table;
        }

        public Field this[string field]
        {
            get
            {
                if (HasField(field))
                    return table[field];
                return new Field(Length);
            }
            set
            {
                table[field] = value;
            }
        }

        /// <summary>
        /// Returns true if this table has this field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HasField(string field)
        {
            return table.ContainsKey(field);
        }

        /// <summary>
        /// Add a field to this table.
        /// </summary>
        /// <param name="field"></param>
        public void AddField(string field)
        {
            table.Add(field, new Field(Length));
        }

        /// <summary>
        /// Returns true if there is a row where "field" has "value"
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(string field, string value)
        {
            return HasField(field) && table[field].Contains(value);
        }

        /// <summary>
        /// Clears the table
        /// </summary>
        public void Clear()
        {
            table = new Dictionary<string, Field>();
        }

        public override string ToString()
        {
            string to_return = "";
            to_return += "Table: " + Length + " rows.\n";
            for (int i = 0; i < Length; i++)
            {
                foreach (string s in table.Keys)
                {
                    to_return += "Field=" + s + " Value=" + table[s][i] + "\n";
                }
            }
            return to_return;
        }
    }
}