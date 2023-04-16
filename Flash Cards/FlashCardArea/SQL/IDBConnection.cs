using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash_Cards.FlashCardData.SQL
{
    internal interface IDBConnection
    {
        public int SQLExecute(string command);
        public SqlConnection SQLConnect(string connection);
    }
}
