using Microsoft.EntityFrameworkCore;
using TravelAgent.Models;

namespace TravelAgent.WebAPI.Contexts
{
    public class TravelAgentDbContext : DbContext
    {
        public TravelAgentDbContext(DbContextOptions<TravelAgentDbContext> options) : base(options) { }

        public DbSet<Flight> Flights => Set<Flight>();
        public DbSet<WebhookSecret> SubscriptionSecrets => Set<WebhookSecret>();

    }
}
