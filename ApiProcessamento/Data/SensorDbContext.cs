using Microsoft.EntityFrameworkCore;
using Shared;

namespace ApiProcessamento.Data
{
    public class SensorDbContext : DbContext
    {
        public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options)
        {
        }

        public DbSet<SensorData> Sensores { get; set; }
    }
}
