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

            //IConnection connection = null;
            //IModel channel = null;

            var queue = "price";
            var exchange = "vtex-seller";


            try
            {
                factory.AutomaticRecoveryEnabled = true;
                factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {

                    //connection = factory.CreateConnection();
                    //channel = connection.CreateModel();

                    channel.ExchangeDeclare(exchange: exchange, type: "direct");

                    //channel.BasicQos

                    //===============================================
                    //create queue if not exists
                    //var _args = new Dictionary<string, object>();
                    //_args.Add("x-message-ttl", 60000);   //expire message
                    //_args.Add("x-expires", 6000000);   //expire queue if not activate miliseconds (15 days is enough)
                    //_args.Add("x-queue-mode", "lazy");   //lazy mode, store message on disk

                    //channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: _args);
                    //===============================================

                    //channel.QueueBind(queue: queue, exchange: exchange, routingKey: "xxx");

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;

                        var msg = Encoding.UTF8.GetString(body);
                        var obj = JsonConvert.DeserializeObject<object>(msg);

                        Console.WriteLine(ea.RoutingKey);

                        //channel.BasicAck(ea.DeliveryTag, false);

                        //if (_delegate(obj).GetAwaiter().GetResult())
                        //{
                        //    channel.BasicAck(ea.DeliveryTag, false);
                        //}
                        //else
                        //{
                        //   channel.BasicNack(ea.DeliveryTag, false, true);
                        //}

                        Console.WriteLine(" [x] Received {0}", msg);
                    };

                    channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                //if (connection != null && connection.IsOpen)
                //{
                //    connection.Close();
                //    connection.Dispose();
                //}

                //if (channel != null && channel.IsOpen)
                //{
                //    channel.Dispose();
                //    channel.Close();
                //}

                throw e;
            }
        }
    }
}
