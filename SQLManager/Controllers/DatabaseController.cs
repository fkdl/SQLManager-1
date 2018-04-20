using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;

namespace SQLManager.Controllers
{
    public class DatabaseController : Controller
    {
        public async Task<IActionResult> Index(string Name)
        {
            var _Columns = new Dictionary<string, List<Tuple<string, string>>>();

            if (Extensions.Connection[0].Equals("SQLServer"))
            {
                if (Extensions.Connection[1].Contains("Initial Catalog="))
                {
                    var _toReplace = Extensions.Connection[1].IndexOf("Initial Catalog=");

                    Extensions.Connection[1] = Extensions.Connection[1].Replace(
                        Extensions.Connection[1].Substring(_toReplace),
                        "Initial Catalog=" + Name);
                }
                else
                {
                    Extensions.Connection[1] += ";Initial Catalog=" + Name;
                }
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

            ViewBag.Title = Name + " tables";
            ViewBag.Type = Extensions.Connection[0];

            return View(_Columns);
        }
    }
}