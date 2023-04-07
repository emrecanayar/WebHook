using Airline.WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Airline.WebAPI.Contexts
{
    //Oluşturulan entitiylerin sql server veri tabanındaki karşılıkları
    public class AirlineDbContext : DbContext
    {
        public AirlineDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Flight> Flights => Set<Flight>();
        public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    }
}