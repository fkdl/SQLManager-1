using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;

namespace SQLManager.Controllers
{
    public class SchemaController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var _Columns = new Dictionary<string, List<Tuple<string, string>>>();

            if (Extensions.Connection[0].Equals("SQLServer"))
            {
                using (var _conn = new SqlConnection(Extensions.Connection[1]))
                {
                    await _conn.OpenAsync();
                    using (var _transaction = _conn.BeginTransaction())
                    {
                        var _Command = _conn.CreateCommand();
                        _Command.Transaction = _transaction;
                        _Command.CommandText = @"SELECT DISTINCT t.TABLE_NAME As 'Table Name',
                                                    ISNULL(Keys.COLUMN_NAME, '0') AS 'Primary Key',
                                                    ISNULL(Cols.DATA_TYPE, '0') AS 'Data Type'
                                                FROM INFORMATION_SCHEMA.TABLES t 
                                                left outer join INFORMATION_SCHEMA.TABLE_CONSTRAINTS Constraints
                                                        on  t.TABLE_NAME = Constraints.Table_name 
                                                        and t.Table_Schema = Constraints.Table_Schema  
                                                        and Constraints.CONSTRAINT_TYPE = 'PRIMARY KEY'       
                                                left outer join INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS Keys 
                                                        ON  Constraints.TABLE_NAME = Keys.TABLE_NAME 
                                                        and Constraints.CONSTRAINT_NAME = Keys.CONSTRAINT_NAME
                                                left outer join INFORMATION_SCHEMA.COLUMNS AS Cols
                                                        ON Keys.TABLE_NAME = Cols.TABLE_NAME
                                                        and Keys.COLUMN_NAME = Cols.COLUMN_NAME";

                        using (var _reader = await _Command.ExecuteReaderAsync())
                        {
                            var _TmpList = new List<Tuple<string, string>>();

                            while (await _reader.ReadAsync())
                            {
                                if (_reader.GetString(1) != "0")
                                {

                                    if (!_Columns.TryAdd(_reader.GetString(0),
                                            new List<Tuple<string, string>>{
                                                Tuple.Create(_reader.GetString(1),
                                                _reader.GetString(2))
                                            })
                                        )
                                    {
                                        _Columns[_reader.GetString(0)].Add(new Tuple<string, string>(
                                            _reader.GetString(1),
                                            _reader.GetString(2)
                                        ));
                                    }
                                }
                                else
                                {
                                    _Columns.Add(_reader.GetString(0), new List<Tuple<string, string>>());
                                }
                            }
                        }
                    }
                }
            }
            else if (Extensions.Connection[0].Equals("SQLite"))
            {
                using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                {
                    var _Tables = new List<string>();
                    await _conn.OpenAsync();
                    using (var _transaction = _conn.BeginTransaction())
                    {
                        var _TableCommand = _conn.CreateCommand();
                        _TableCommand.Transaction = _transaction;
                        _TableCommand.CommandText = @"SELECT tbl_name
                                                 FROM sqlite_master
                                                 WHERE type = 'table'
                                                 AND tbl_name NOT LIKE '%sqlite%'";
                        using (var _reader = await _TableCommand.ExecuteReaderAsync())
                        {
                            while (await _reader.ReadAsync())
                            {
                                _Tables.Add(_reader.GetString(0));
                            }
                        }

                        foreach (var _element in _Tables)
                        {
                            var _TmpList = new List<Tuple<string, string>>();

                            var _ColumnsCommand = _conn.CreateCommand();
                            _ColumnsCommand.Transaction = _transaction;
                            _ColumnsCommand.CommandText = @"PRAGMA table_info('" + _element + "');";

                            using (var _reader = await _ColumnsCommand.ExecuteReaderAsync())
                            {
                                while (await _reader.ReadAsync())
                                {
                                    if (_reader.GetInt32(5) == 1)
                                    {
                                        _TmpList.Add(new Tuple<string, string>(
                                            _reader.GetString(1),
                                            _reader.GetString(2)
                                        ));
                                    }
                                }

                                _Columns.Add(_element, _TmpList);
                            }
                        }
                    }
                }
            }

            return View(_Columns);
        }

        [HttpPost]
        public async Task<int> Create(string TableName, string[] FieldData)
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
                TempData["Error"] = ex.Message;
            }

            return 0;
        }
    }
}