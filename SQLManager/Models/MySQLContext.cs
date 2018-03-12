using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore;

namespace SQLManager.Models
{
    public class MySQLContext : DbContext
    {
        public string ConnectionString { get; set; }

        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options)
        {
        }
    }
}