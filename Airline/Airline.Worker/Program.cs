//Webhook uygulamasýnýn ikinci ayaðý olan ve Message Bus ile gönderilen mesajlarý iþleyip ilgili istemcileri bilgilendirecek console projesini oluþturalým ve gerekli NuGet paketlerini kuralým.
using Airline.Worker.Contexts;
using Microsoft.EntityFrameworkCore;
using Worker.App;
using Worker.Client;

IHost host = Host.CreateDefaultBuilder(args)
 .ConfigureServices((context, services) =>
 {
     services.AddLogging();
     services.AddDbContext<AirlineDbContext>(o => o.UseSqlServer(context.Configuration.GetConnectionString("SqlServer")));
     services.AddScoped<IWebhookClient, WebhookHttpClient>();
     services.AddHttpClient();
     services.AddScoped<IAppHost, AppHost>();
 }).Build();

using var scope = host.Services.CreateScope();
scope.ServiceProvider.GetService<IAppHost>()?.Run();