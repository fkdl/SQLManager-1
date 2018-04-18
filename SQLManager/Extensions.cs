using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;

namespace SQLManager
{
    public abstract class Extensions
    {
        /// <summary>
        /// [0] --> Connection Type  [1] --> Connection String
        /// </summary>
        public static string[] Connection = new string[2];
    }
}