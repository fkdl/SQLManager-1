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
            string _PrimaryKey = "";


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

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            while (await _reader.ReadAsync())
                            {
                                _Data.Columns.Add(_reader.GetString(1));

                                if (_reader.GetInt32(5) == 1)
                                {
                                    _PrimaryKey = _reader.GetString(1);
                                }
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

                    using (var _transaction = _conn.BeginTransaction())
                    {
                        var _Command = _conn.CreateCommand();
                        _Command.Transaction = _transaction;
                        _Command.CommandText = @"SELECT * FROM sqlite_sequence WHERE name = '" + Name + "'";

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            if (_reader.HasRows && _PrimaryKey.Length > 0)
                            {
                                ViewBag.AutoInc = _PrimaryKey;
                            }
                        }
                    }
                }
            }

            ViewBag.Title = "View " + Name + " data";
            ViewBag.Table = Name;

            return View(_Data);
        }

        [HttpPost]
        public async void Add(string InsertData, string FieldNames, string TableName)
        {
            if (Extensions.Connection[0].Equals("SQLite"))
            {
                using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                {
                    await _conn.OpenAsync();

                    using (var _transaction = _conn.BeginTransaction())
                    {
                        var _Command = _conn.CreateCommand();
                        _Command.Transaction = _transaction;
                        _Command.CommandText = @"INSERT INTO " + TableName + "(" + FieldNames + ") VALUES (" + InsertData + ")";

                        await _Command.ExecuteNonQueryAsync();

                        _transaction.Commit();
                    }
                }
            }
        }
    }
}