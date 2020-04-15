using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer._2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            IConnection connection = null;
            IModel channel = null;

            var queue = "price";
            var exchange = "vtex-seller";

            try
            {
                factory.AutomaticRecoveryEnabled = true;
                factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: exchange, type: "direct");

                //channel.QueueBind(queue: queue, exchange: exchange, routingKey: "xxx");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;

                    var msg = Encoding.UTF8.GetString(body);
                    var obj = JsonConvert.DeserializeObject<object>(msg);

                    channel.BasicAck(ea.DeliveryTag, false);

                    //if (_delegate(obj).GetAwaiter().GetResult())
                    //{
                    //    channel.BasicAck(ea.DeliveryTag, false);
                    //}
                    //else
                    //{
                    //    channel.BasicNack(ea.DeliveryTag, false, true);
                    //}

                    Console.WriteLine(" [x] Received {0}", msg);
                };

                channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
            }
            catch (Exception e)
            {
                if (connection != null && connection.IsOpen)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (channel != null && channel.IsOpen)
                {
                    channel.Dispose();
                    channel.Close();
                }

                throw e;
            }
        }
    }
}
