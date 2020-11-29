using Microsoft.EntityFrameworkCore;
using ParkingLotApi.Entity;

namespace ParkingLotApi.Repository
{
    public class ParkingLotContext : DbContext
    {
        public ParkingLotContext(DbContextOptions<ParkingLotContext> options)
            : base(options)
        {
        }

        public DbSet<ParkingLotEntity> ParkingLots { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
    }
}