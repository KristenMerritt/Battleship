using Microsoft.EntityFrameworkCore;

namespace Battleship.Models
{
    public class BattleshipContext : DbContext
    {
        public DbSet<db_Player> Players { get; set; }
        public DbSet<db_Chat> Chats { get; set; }
        public DbSet<db_Board> Boards { get; set; }
        public DbSet<db_Game> Games { get; set; }
        public DbSet<db_Challenge> Challenges { get; set; }
        public DbSet<db_Ship> Ships { get; set; }
        public DbSet<db_ShipLocation> ShipLocations { get; set; }
        public DbSet<db_ShipType> ShipTypes { get; set; }
        public DbSet<db_Shot> Shots { get; set; }

        public BattleshipContext(DbContextOptions<BattleshipContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<db_Player>().ToTable("player");
            modelBuilder.Entity<db_Chat>().ToTable("chat");
            modelBuilder.Entity<db_Board>().ToTable("board");
            modelBuilder.Entity<db_Game>().ToTable("game");
            modelBuilder.Entity<db_Challenge>().ToTable("challenge");
            modelBuilder.Entity<db_Ship>().ToTable("ship");
            modelBuilder.Entity<db_ShipLocation>().ToTable("ship_location");
            modelBuilder.Entity<db_ShipType>().ToTable("ship_type");
            modelBuilder.Entity<db_Shot>().ToTable("shot");
        }
    }
}
