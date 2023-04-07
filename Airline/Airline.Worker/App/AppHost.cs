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

    //Publisher dan gelen bilgiyi consume ederek webhookClient a g�nderilir. WebHookClient  ta ilgili mesaj� subscriber olmus olan clientlara iletecektir.
    public void Run()
    {
        ConnectionFactory factory = new();
        factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

        //Ba�lant�y� aktlifle�tirme ve kanal a�ma.
        using IConnection connection = factory.CreateConnection(); //Ba�lant� olu�turma
        using IModel channel = connection.CreateModel(); // Olu�turulan bu ba�lant� �zerinden bir kanal olu�turduk.

        //�imdi burada �ncelikle Publisher da yer alan exchange i ayn�s�n� tan�mlamam�z gerekiyor.
        channel.ExchangeDeclare(
            exchange: "fanout-exchange-price",
            type: ExchangeType.Fanout
            );

        var queueName = channel.QueueDeclare().QueueName;

        //Olu�turdu�umuz kuyru�u art�k bind etmemiz gerekmektedir
        channel.QueueBind(queue: queueName, exchange: "fanout-exchange-price", routingKey: string.Empty);

        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
        //Art�k Publisher dan gelen mesajlar� okuma zaman� geldi. �imdi bunu olu�tural�m.
        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer); // autoAck false yapt�k ��nk� mesaj� i�ledikten sonra kuyruktan d��mesini gerekti�ini ben y�netmek istiyorum.
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

            //BasicAck metodu ile mesaj�n ba�ar�yla i�lendi�ini RabbitMQ ya bildiriyoruz ve art�k kuyruktan silinmesi gerekti�ini belirtiyoruz.

            channel.BasicAck(args.DeliveryTag, multiple: false);
            //multiple parametresi e�er true olursa buna dair di�er mesajlar� da onayland���n� bildirmi� olur. Fakat false olursa sadece spesifik olarak ilgili mesaj�n sonland�r�ld��� anlam� kat�l�r.
        };


        Console.Read();
    }

    private NotificationMessage? getMessage(byte[] bytes)
    {
        var payload = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<NotificationMessage>(payload);
    }
}