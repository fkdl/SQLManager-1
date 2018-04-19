using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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

                if (Extensions.Connection[0].Equals("SQLServer"))
                {
                    using (var _conn = new SqlConnection(Extensions.Connection[1]))
                    {
                        await _conn.OpenAsync();
                        using (var _transaction = _conn.BeginTransaction())
                        {
                            var _Command = _conn.CreateCommand();
                            _Command.Transaction = _transaction;
                            _Command.CommandText = "CREATE TABLE " + TableName + " (";

                            foreach (var _element in FieldData)
                            {
                                var _line = _element.Split(";");

                                if (_line[4].Length > 0)
                                {
                                    _Command.CommandText += _line[2] + " " + _line[3] + "(" + _line[4] + ")";
                                }
                                else
                                {
                                    _Command.CommandText += _line[2] + " " + _line[3];
                                }

                                if (_line[0] == "true" && _line[1] == "false")
                                {
                                    _Command.CommandText += " PRIMARY KEY, ";
                                }
                                else if (_line[1] == "true")
                                {
                                    _Command.CommandText += " IDENTITY(1,1) PRIMARY KEY, ";
                                }
                                else
                                {
                                    _Command.CommandText += ", ";
                                }
                            }

                            _Command.CommandText = _Command.CommandText.Remove(_Command.CommandText.Length - 2) + ")";

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
                            _Command.CommandText = "CREATE TABLE " + TableName + " (";

                            foreach (var _element in FieldData)
                            {
                                var _line = _element.Split(";");

                                if (_line[0] == "true" && _line[1] == "false")
                                {
                                    _Command.CommandText += _line[2] + " " + _line[3] + " PRIMARY KEY, ";
                                }
                                else if (_line[1] == "true")
                                {
                                    _Command.CommandText += _line[2] + " " + _line[3] + " PRIMARY KEY AUTOINCREMENT, ";
                                }
                                else
                                {
                                    _Command.CommandText += _line[2] + " " + _line[3] + ", ";
                                }
                            }

                            _Command.CommandText = _Command.CommandText.Remove(_Command.CommandText.Length - 2) + ")";

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