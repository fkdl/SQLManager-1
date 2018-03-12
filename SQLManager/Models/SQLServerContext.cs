using Microsoft.EntityFrameworkCore;

namespace SQLManager.Models
{
    public class SQLServerContext : DbContext
    {
        public SQLServerContext(DbContextOptions<SQLServerContext> options) : base(options)
        {
        }
    }
}