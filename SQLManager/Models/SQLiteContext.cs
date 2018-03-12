using Microsoft.EntityFrameworkCore;

namespace SQLManager.Models
{
    public class SQLiteContext : DbContext
    {
        public string ConnectionString { get; set; }
        public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options)
        {
        }
    }
}