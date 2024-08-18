using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Models;

namespace SensorMonitoring.Data
{
    public class SensorDataContext : DbContext
    {
        public SensorDataContext(DbContextOptions<SensorDataContext> options) : base(options)
        {
        }

        public DbSet<SensorData> SensorData { get; set; }
        public DbSet<SetorEquipamento> SetoresEquipamentos { get; set; }
        public DbSet<Sensor> Sensores { get; set; }
    }
}
