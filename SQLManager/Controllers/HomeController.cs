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

                return RedirectToAction("Index", "Schema");
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
                var _connection = "Data Source=" + _server + ";Initial Catalog=" + _db +
                    ";User id=" + _user + ";Password=" + _pass + ";";

                using (SqlConnection _conn = new SqlConnection(_connection))
                {
                    await _conn.OpenAsync();
                }

                Extensions.Connection[0] = "SQLServer";
                Extensions.Connection[1] = _connection;
                return RedirectToAction("Index", "Schema");
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
