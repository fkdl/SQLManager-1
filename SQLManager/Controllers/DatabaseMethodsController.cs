using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace SQLManager.Controllers
{
    public class DatabaseMethodsController
    {
        [HttpPost]
        public async Task<string> Create(string TableName, string[] FieldData)
        {
            try
            {
                if (TableName == null)
                {
                    throw new System.Exception("No valid Table Name provided");
                }

                var _pk = new List<string>();

                var _cmd = "CREATE TABLE " + TableName + " (";

                foreach (var _element in FieldData)
                {
                    var _line = _element.Split(";");

                    if (_line[2].Length > 0)
                    {
                        if (_line[4].Length > 0)
                        {
                            _cmd += _line[2] + " " + _line[3] + "(" + _line[4] + ")";
                        }
                        else
                        {
                            _cmd += _line[2] + " " + _line[3];
                        }

                        if (_line[0].Equals("true") && _line[1].Equals("false"))
                        {
                            if (Extensions.Connection[0].Equals("SQLServer") ||
                                Extensions.Connection[0].Equals("MySQL"))
                            {
                                _cmd += ", ";
                                _pk.Add(_line[2]);
                            }
                            else if (Extensions.Connection[0].Equals("SQLite"))
                            {
                                _cmd += "PRIMARY KEY, ";
                            }
                        }
                        else if (_line[1].Equals("true"))
                        {
                            if (Extensions.Connection[0].Equals("SQLServer"))
                            {
                                _cmd += " IDENTITY(1,1), ";
                                _pk.Add(_line[2]);
                            }
                            else if (Extensions.Connection[0].Equals("SQLite"))
                            {
                                _cmd += " PRIMARY KEY AUTOINCREMENT, ";
                            }
                            else if (Extensions.Connection[0].Equals("MySQL"))
                            {
                                _cmd += " AUTO_INCREMENT, ";
                                _pk.Add(_line[2]);
                            }
                        }
                        else
                        {
                            _cmd += ", ";
                        }
                    }
                }

                if (_pk.Count > 0)
                {
                    _cmd += " PRIMARY KEY(";
                    foreach (var _element in _pk)
                    {
                        _cmd += _element + ", ";
                    }

                    _cmd = _cmd.Remove(_cmd.Length - 2) + "), ";
                }

                _cmd = _cmd.Remove(_cmd.Length - 2) + ")";

                if (Extensions.Connection[0].Equals("SQLServer"))
                {
                    using (var _conn = new SqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();
                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            _Command.CommandText = _cmd;

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

                            _Command.CommandText = _cmd;

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
                else if (Extensions.Connection[0].Equals("MySQL"))
                {
                    using (var _conn = new MySqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = await _conn.BeginTransactionAsync())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            _Command.CommandText = _cmd;

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "OK";
        }

        [HttpPost]
        public async Task<string> Drop(string Table)
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
                            _Command.CommandText = @"DROP TABLE " + Table;

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
                            _Command.CommandText = @"DROP TABLE " + Table;

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
                else if (Extensions.Connection[0].Equals("MySQL"))
                {
                    using (var _conn = new MySqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = await _conn.BeginTransactionAsync())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;
                            _Command.CommandText = @"DROP TABLE " + Table;

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "OK";
        }

        [HttpPost]
        public async Task<string> Rename(string OldName, string NewName)
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
                            _Command.CommandText = @"EXEC sp_rename @oldTable, @newTable";
                            _Command.Parameters.AddWithValue("@oldTable", OldName);
                            _Command.Parameters.AddWithValue("@newTable", NewName);

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
                            _Command.CommandText = @"ALTER TABLE " + OldName + " RENAME TO " + NewName.Replace(";", "");

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
                else if (Extensions.Connection[0].Equals("MySQL"))
                {
                    using (var _conn = new MySqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();

                        using (var _transaction = await _conn.BeginTransactionAsync())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;

                            var _cmdBuilder = new MySqlCommandBuilder();

                            _Command.CommandText = @"RENAME TABLE " + _cmdBuilder.QuoteIdentifier(OldName) + " TO " +
                                _cmdBuilder.QuoteIdentifier(NewName);

                            await _Command.ExecuteNonQueryAsync();

                            _transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "OK";
        }
    }
}