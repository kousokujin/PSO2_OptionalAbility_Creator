using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace PSO2_OptionalAbility_Creator
{
    public abstract class MemorySQL_Load
    {
        SQLiteConnectionStringBuilder sqlitedata;
        SQLiteConnection sql_connection;
        protected MemorySQL_Load(string datasource, string init_sql)
        {
            sqlitedata = new SQLiteConnectionStringBuilder() { DataSource = datasource };
            connect();
            init();
            load_data(init_sql);
        }

        public void connect()
        {
            sql_connection = new SQLiteConnection(sqlitedata.ToString());
        }

        public void load_data(string sql)
        {
            sql_connection.Open();
            using (var cmd = new SQLiteCommand(sql_connection))
            {

                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        load(reader);
                    }
                }
            }

            sql_connection.Close();
        }

        abstract public void load(SQLiteDataReader data);
        abstract public void init();
    }
}
