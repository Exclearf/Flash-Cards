using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash_Cards.FlashCardData.SQL
{
    internal class DBConnection : IDBConnection
    {
        private SqlConnection _connection { get; set; }

        public SqlConnection SQLConnect(string c)
        {
            _connection = new SqlConnection(c);
            _connection.Open();
            return _connection;
        }

        public int SQLExecute(string cmd_txt)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = cmd_txt;
            //try
            //{
                return cmd.ExecuteNonQuery();
            //}
            //catch { return -1; }
        }
    }
}
