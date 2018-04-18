using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace SQLManager.Controllers
{
    public class DatabaseController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "All Databases";
            using (var _conn = new SqlConnection(Extensions.Connection[1]))
            {
                await _conn.OpenAsync();
                using (var _Command = _conn.CreateCommand())
                {
                    _Command.CommandText = @"SELECT name FROM master.dbo.sysdatabases";
                    using (var _reader = await _Command.ExecuteReaderAsync())
                    {
                        var _databases = new List<string>();

                        while (await _reader.ReadAsync())
                        {
                            _databases.Add(_reader.GetString(0));
                        }

                        return View(_databases);
                    }
                }
            }
        }
    }
}