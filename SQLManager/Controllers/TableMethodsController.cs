using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace SQLManager.Controllers
{
    public class TableMethodsController : Controller
    {
        [HttpPost]
        public async Task<string> Add(string[] InsertData, string FieldNames, string TableName)
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
                return ex.Message;
            }

            return "OK";
        }

        [HttpPost]
        public async Task<string> Edit(string[] InsertData, string FieldNames, string TableName, string[] PrimaryKey)
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
                return ex.Message;
            }

            return "OK";
        }

        [HttpPost]
        public async Task<string> Remove(string[] RemoveKey, string TableName)
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
                return ex.Message;
            }

            return "OK";
        }
    }
}