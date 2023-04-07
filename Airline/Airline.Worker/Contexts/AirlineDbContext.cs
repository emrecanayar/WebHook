using Airline.Worker.Entities;
using Microsoft.EntityFrameworkCore;

namespace Airline.Worker.Contexts
{
    public class AirlineDbContext : DbContext
    {
        public AirlineDbContext(DbContextOptions<AirlineDbContext> options) : base(options) { }

        public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    }
}
