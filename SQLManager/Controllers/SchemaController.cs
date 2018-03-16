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
            var _Tables = new List<string>();
            var _Columns = new Dictionary<string, List<Tuple<string, string, string>>>();

            if (Extensions.Connection[0].Equals("SQLite"))
            {
                using (var _conn = new SqliteConnection(Extensions.Connection[1]))
                {
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
                            var _TmpList = new List<Tuple<string, string, string>>();

                            var _ColumnsCommand = _conn.CreateCommand();
                            _ColumnsCommand.Transaction = _transaction;
                            _ColumnsCommand.CommandText = @"PRAGMA table_info('" + _element + "');";

                            using (var _reader = await _ColumnsCommand.ExecuteReaderAsync())
                            {
                                while (await _reader.ReadAsync())
                                {
                                    _TmpList.Add(new Tuple<string, string, string>(
                                        _reader.GetString(1),
                                        _reader.GetString(2),
                                        _reader.GetString(5)
                                    ));
                                }

                                _Columns.Add(_element, _TmpList);
                            }
                        }
                    }
                }
            }
            
            return View(_Columns);
        }
    }
}