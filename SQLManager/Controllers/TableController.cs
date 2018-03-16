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
                    await _conn.OpenAsync();

                    using (var _transaction = _conn.BeginTransaction())
                    {
                        var _Command = _conn.CreateCommand();
                        _Command.Transaction = _transaction;
                        _Command.CommandText = @"PRAGMA table_info('" + Name + "');";
                        // _Command.Parameters.AddWithValue("$table", Name);

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            while (await _reader.ReadAsync())
                            {
                                _Data.Columns.Add(_reader.GetString(1));
                            }
                        }
                    }

                    using (var _transaction = _conn.BeginTransaction())
                    {
                        var _Command = _conn.CreateCommand();
                        _Command.Transaction = _transaction;
                        _Command.CommandText = @"SELECT * FROM " + Name;

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                                var _ColumnData = new string[_reader.FieldCount];

                                while (await _reader.ReadAsync())
                                {
                                    for (int i = 0; i < _reader.FieldCount; i++)
                                    {
                                        _ColumnData[i] = _reader[i].ToString();
                                    }

                                    _Data.Rows.Add(_ColumnData);
                                }
                        }
                    }
                }
            }

            return View(_Data);
        }
    }
}