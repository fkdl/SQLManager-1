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
            List<string> _PrimaryKey = new List<string>();

            if (Extensions.Connection[0].Equals("SQLServer"))
            {
                using (var _conn = new SqlConnection(Extensions.Connection[1]))
                {
                    await _conn.OpenAsync();
                    using (var _Command = _conn.CreateCommand())
                    {
                        _Command.CommandText = @"   SELECT COLUMN_NAME
                                                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                                                    WHERE OBJECTPROPERTY(
                                                        OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)),
                                                        'IsPrimaryKey'
                                                    ) = 1
                                                    AND TABLE_NAME = '" + Name + "'";

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            while (await _reader.ReadAsync())
                            {
                                _PrimaryKey.Add(_reader.GetString(0));
                            }
                        }
                    }

                    using (var _Command = _conn.CreateCommand())
                    {
                        _Command.CommandText = @"   SELECT COLUMN_NAME
                                                    FROM INFORMATION_SCHEMA.COLUMNS
                                                    WHERE COLUMNPROPERTY(
                                                        OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME),
                                                        COLUMN_NAME,
                                                        'IsIdentity'
                                                    ) = 1
                                                    AND TABLE_NAME = '" + Name + "'";

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            while (await _reader.ReadAsync())
                            {
                                ViewBag.AutoInc = _reader.GetString(0);
                            }
                        }
                    }

                    using (var _Command = _conn.CreateCommand())
                    {
                        _Command.CommandText = @"SELECT * FROM " + Name;

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            _Data.Load(_reader);
                        }
                    }
                }
            }
            else if (Extensions.Connection[0].Equals("SQLite"))
            {
                using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                {
                    await _conn.OpenAsync();
                    using (var _Command = _conn.CreateCommand())
                    {
                        _Command.CommandText = @"PRAGMA table_info('" + Name + "');";

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            while (await _reader.ReadAsync())
                            {
                                _Data.Columns.Add(_reader.GetString(1));

                                if (_reader.GetInt32(5) == 1)
                                {
                                    _PrimaryKey.Add(_reader.GetString(1));
                                }
                            }
                        }
                    }

                    using (var _Command = _conn.CreateCommand())
                    {
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

                    using (var _Command = _conn.CreateCommand())
                    {
                        _Command.CommandText = @"SELECT * FROM sqlite_sequence WHERE name = '" + Name + "'";

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            if (_reader.HasRows && _PrimaryKey.Count > 0)
                            {
                                //Auto increment must be first
                                ViewBag.AutoInc = _PrimaryKey[0];
                            }
                        }
                    }
                }
            }

            ViewBag.Title = "View " + Name + " data";
            ViewBag.Table = Name;
            ViewBag.PrimaryKey = _PrimaryKey;

            return View(_Data);
        }

        [HttpPost]
        public async Task<int> Add(string[] InsertData, string FieldNames, string TableName)
        {
            try
            {
                if (Extensions.Connection[0].Equals("SQLServer"))
                {
                    using (var _conn = new SqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            var _parameters = "";

                            for (int i = 0; i < InsertData.Length; i++)
                            {
                                _parameters += @"@param" + i + ", ";
                                _Command.Parameters.AddWithValue("@param" + i, InsertData[i]);
                            }

                            _Command.CommandText = @"INSERT INTO " + TableName + "(" +
                                FieldNames.Remove(FieldNames.Length - 2) + ") VALUES (" +
                                _parameters.Remove(_parameters.Length - 2) + ")";

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
                else if (Extensions.Connection[0].Equals("SQLite"))
                {
                    using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            var _parameters = "";

                            for (int i = 0; i < InsertData.Length; i++)
                            {
                                _parameters += @"$param" + i + ", ";
                                _Command.Parameters.AddWithValue("$param" + i, InsertData[i]);
                            }

                            _Command.CommandText = @"INSERT INTO " + TableName + "(" +
                                FieldNames.Remove(FieldNames.Length - 2) + ") VALUES (" +
                                _parameters.Remove(_parameters.Length - 2) + ")";

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return 0;
        }

        [HttpPost]
        public async Task<int> Edit(string[] InsertData, string FieldNames, string TableName, string[] PrimaryKey)
        {
            try
            {
                if (Extensions.Connection[0].Equals("SQLServer"))
                {
                    using (var _conn = new SqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            var _fields = FieldNames.Remove(FieldNames.Length - 2).Split(", ");
                            var _command = "UPDATE " + TableName + " SET ";

                            for (int i = 0; i < InsertData.Length; i++)
                            {
                                _Command.Parameters.AddWithValue("@param" + i, InsertData[i]);
                                _command += _fields[i] + " = @param" + i + ", ";
                            }

                            _Command.Parameters.AddWithValue("@key", PrimaryKey[1]);

                            _command = _command.Remove(_command.Length - 2) + " WHERE " + PrimaryKey[0] + " = @key";

                            _Command.CommandText = _command;

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
                else if (Extensions.Connection[0].Equals("SQLite"))
                {
                    using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            var _fields = FieldNames.Remove(FieldNames.Length - 2).Split(", ");
                            var _command = "UPDATE " + TableName + " SET ";

                            for (int i = 0; i < InsertData.Length; i++)
                            {
                                _Command.Parameters.AddWithValue("$param" + i, InsertData[i]);
                                _command += _fields[i] + " = $param" + i + ", ";
                            }

                            _Command.Parameters.AddWithValue("$key", PrimaryKey[1]);

                            _command = _command.Remove(_command.Length - 2) + " WHERE " + PrimaryKey[0] + " = $key";

                            _Command.CommandText = _command;

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return 0;
        }

        [HttpPost]
        public async Task<int> Remove(string[] RemoveKey, string TableName)
        {
            try
            {
                if (Extensions.Connection[0].Equals("SQLServer"))
                {
                    using (var _conn = new SqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            _Command.Parameters.AddWithValue("@key", RemoveKey[1]);
                            _Command.CommandText = "DELETE FROM " + TableName + " WHERE " + RemoveKey[0] + " = @key";

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
                else if (Extensions.Connection[0].Equals("SQLite"))
                {
                    using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            _Command.Parameters.AddWithValue("$key", RemoveKey[1]);
                            _Command.CommandText = "DELETE FROM " + TableName + " WHERE " + RemoveKey[0] + " = $key";

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return 0;
        }
    }
}