using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SQLManager.Models;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace SQLManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SQLIteContext(string _server)
        {
            if (System.IO.File.Exists(_server))
            {
                Extensions.Connection[0] = "SQLite";
                Extensions.Connection[1] = "Data Source=" + _server;

                return RedirectToAction("Index", "Database", new { Name = _server });
            }
            else
            {
                TempData["Error"] = "Database file not found";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLServerContext(string _server, string _db, string _user, string _pass)
        {
            try
            {
                var _connection = "";

                if (_db != null)
                {
                    _connection = "Data Source=" + _server + ";User id=" + _user + ";Password=" +
                        _pass + ";Initial Catalog=" + _db;
                }
                else
                {
                    _connection = "Data Source=" + _server + ";User id=" + _user + ";Password=" + _pass;
                }

                using (SqlConnection _conn = new SqlConnection(_connection))
                {
                    await _conn.OpenAsync();
                }

                Extensions.Connection[0] = "SQLServer";
                Extensions.Connection[1] = _connection;

                if (_db != null)
                {
                    return RedirectToAction("Index", "Database", new { Name = _db });
                }
                else
                {
                    return RedirectToAction("Index", "Databases");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MySQLContext(string _server, string _db, string _user, string _pass)
        {
            try
            {
                var _connection = new MySqlConnectionStringBuilder();

                var _sp = _server.Split(':');

                _connection.Server = _sp[0];
                if (_sp.Length > 1)
                {
                    _connection.Port = uint.Parse(_sp[1]);
                }
                else
                {
                    _connection.Port = 3306;
                }
                _connection.UserID = _user;
                _connection.Password = _pass;
                
                if (_db != null)
                {
                    _connection.Database = _db;
                }

                using (var _conn = new MySqlConnection(_connection.ToString()))
                {
                    await _conn.OpenAsync();
                }

                Extensions.Connection[0] = "MySQL";
                Extensions.Connection[1] = _connection.ToString();
                var _t = _connection.ToString();

                if (_db != null)
                {
                    return RedirectToAction("Index", "Database", new { Name = _db });
                }
                else
                {
                    return RedirectToAction("Index", "Databases");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
