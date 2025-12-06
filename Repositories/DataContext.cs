using Microsoft.EntityFrameworkCore;

namespace KiwdyAPI.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }
    }
}
