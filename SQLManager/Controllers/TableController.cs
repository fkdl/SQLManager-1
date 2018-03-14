using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace SQLManager.Controllers
{
    public class TableController : Controller
    {
        public async Task<IActionResult> Index(string Name)
        {
            var _Data = new DataTable();

            if (Extensions.Connection[0].Equals("SQLite"))
            {
                using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                {
                    var _Command = _conn.CreateCommand();
                    _Command.CommandText = @"Select * from $db";
                    _Command.Parameters.AddWithValue("$db", Name);

                    await _conn.OpenAsync();
                    using (var _reader = _Command.ExecuteReader())
                    {
                        _Data.Load(_reader);
                    }
                }
            }

            return View(_Data);
        }
    }
}