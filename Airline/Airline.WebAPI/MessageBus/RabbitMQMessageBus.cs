using Airline.WebAPI.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Airline.WebAPI.MessageBus
{
    //Fiyat bilgisinin güncellendiği bilgisini subscribe olanlara yayınalayacak olan bus tasarımı
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly ILogger<RabbitMQMessageBus> _logger;

        public RabbitMQMessageBus(ILogger<RabbitMQMessageBus> logger)
        {
            _logger = logger;
        }
        public void Publish(NotificationMessage notificationMessage)
        {
            //Bağlantı bilgilerini oluşturma
            ConnectionFactory factory = new();
            factory.Uri = new("amqps://ozlhavpw:dCQqC5opP19AijsrsJH4o-yJmql2b7pM@hawk.rmq.cloudamqp.com/ozlhavpw");

            //Bağlantıyı aktlifleştirme ve kanal açma.
            using IConnection connection = factory.CreateConnection(); //Bağlantı oluşturma
            using IModel channel = connection.CreateModel(); // Oluşturulan bu bağlantı üzerinden bir kanal oluşturduk.

            //Burada kanal üzerinden fanout exchange tanımladık. Tanımladığımız exhange in adını ve tipini burada bu şekilde tanımlıyoruz.
            channel.ExchangeDeclare(
                exchange: "fanout-exchange-price",
                type: ExchangeType.Fanout
                );

            //Mesajı uygun formatada getirdik.
            byte[] message = getMessage(notificationMessage);

            //Mesajı publish ettik.
            channel.BasicPublish(exchange: "fanout-exchange-price", routingKey: string.Empty, basicProperties: null, body: message);
        }

        private byte[] getMessage(NotificationMessage notificationMessage)
        {
            var message = JsonSerializer.Serialize(notificationMessage);
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
