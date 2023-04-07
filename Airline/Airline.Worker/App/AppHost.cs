using Airline.Worker.Contexts;
using Airline.Worker.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Worker.Client;
using Worker.Models;

namespace Worker.App;

public class AppHost : IAppHost
{
    private readonly IWebhookClient _client;
    private readonly AirlineDbContext _context;
    private readonly ILogger<AppHost> _logger;

    public AppHost(ILogger<AppHost> logger, AirlineDbContext context, IWebhookClient client)
    {
        _logger = logger;
        _context = context;
        _client = client;
    }

    //Publisher dan gelen bilgiyi consume ederek webhookClient a gönderilir. WebHookClient  ta ilgili mesajý subscriber olmus olan clientlara iletecektir.
    public void Run()
    {
        ConnectionFactory factory = new();
        factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

        //Baðlantýyý aktlifleþtirme ve kanal açma.
        using IConnection connection = factory.CreateConnection(); //Baðlantý oluþturma
        using IModel channel = connection.CreateModel(); // Oluþturulan bu baðlantý üzerinden bir kanal oluþturduk.

        //Þimdi burada öncelikle Publisher da yer alan exchange i aynýsýný tanýmlamamýz gerekiyor.
        channel.ExchangeDeclare(
            exchange: "fanout-exchange-price",
            type: ExchangeType.Fanout
            );

        var queueName = channel.QueueDeclare().QueueName;

        //Oluþturduðumuz kuyruðu artýk bind etmemiz gerekmektedir
        channel.QueueBind(queue: queueName, exchange: "fanout-exchange-price", routingKey: string.Empty);

        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
        //Artýk Publisher dan gelen mesajlarý okuma zamaný geldi. Þimdi bunu oluþturalým.
        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer); // autoAck false yaptýk çünkü mesajý iþledikten sonra kuyruktan düþmesini gerektiðini ben yönetmek istiyorum.
        _logger.LogInformation("Listening on the message bus...");

        consumer.Received += async (sender, args) =>
        {
            _logger.LogInformation("Event is triggered!");

            NotificationMessage? message = getMessage(args.Body.ToArray());
            if (message is null) return;

            foreach (WebhookSubscription? whs in _context.WebhookSubscriptions.Where(w => w.WebhookType == message.WebhookType))
            {
                var webhookToSend = new ChangePayload
                {
                    NewPrice = message.NewPrice,
                    Code = message.Code,
                };

                await _client.SendAsync(whs.WebhookUri, webhookToSend, new Dictionary<string, string>
                {
                    { "Secret", whs.Secret.ToString() },
                    { "Publisher", whs.WebhookPublisher },
                    { "Event-Type", "PriceChange"},
                });
            }

            //BasicAck metodu ile mesajýn baþarýyla iþlendiðini RabbitMQ ya bildiriyoruz ve artýk kuyruktan silinmesi gerektiðini belirtiyoruz.

            channel.BasicAck(args.DeliveryTag, multiple: false);
            //multiple parametresi eðer true olursa buna dair diðer mesajlarý da onaylandýðýný bildirmiþ olur. Fakat false olursa sadece spesifik olarak ilgili mesajýn sonlandýrýldýðý anlamý katýlýr.
        };


        Console.Read();
    }

    private NotificationMessage? getMessage(byte[] bytes)
    {
        var payload = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<NotificationMessage>(payload);
    }
}