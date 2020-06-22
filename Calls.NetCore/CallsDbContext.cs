using Microsoft.EntityFrameworkCore;

namespace Calls.NetCore
{
    public class CallsDbContext : DbContext
    {
        //public CallsDbContext(DbContextOptions<CallsDbContext> options) : base(options)
        //{
        //}

        public DbSet<Call> Calls { get; set; }

        public DbSet<PhoneNumber> PhoneNumbers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=Calls;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
        }
    }
}
