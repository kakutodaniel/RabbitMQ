using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumerWindowsService._2
{
    public class Process : IHostedService, IDisposable
    {
        //private Timer _timer;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //_timer = new Timer(
            //    (e) => Run(),
            //    null,
            //    TimeSpan.Zero,
            //    TimeSpan.FromMinutes(1));
            Run();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //_timer?.Change(Timeout.Infinite, 0);
            Dispose();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //_timer?.Dispose();
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }



        private void Run()
        {
            _factory = new ConnectionFactory
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

            _factory.AutomaticRecoveryEnabled = true;
            _factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            //distributed messages according to consumer is idle
            //1 at a time
            _channel.BasicQos(0, 1, false);


            //connection = factory.CreateConnection();
            //channel = connection.CreateModel();

            _channel.ExchangeDeclare(exchange: exchange, type: "direct");

            //===============================================
            //create queue if not exists
            //var _args = new Dictionary<string, object>();
            //_args.Add("x-message-ttl", 60000);   //expire message
            //_args.Add("x-expires", 6000000);   //expire queue if not activate miliseconds (15 days is enough)
            //_args.Add("x-queue-mode", "lazy");   //lazy mode, store message on disk

            //channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: _args);
            //===============================================

            //channel.QueueBind(queue: queue, exchange: exchange, routingKey: "xxx");

            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body;

                var msg = Encoding.UTF8.GetString(body);
                var obj = JsonConvert.DeserializeObject<object>(msg);

                //Console.WriteLine(ea.RoutingKey);

                File.AppendAllText("C:\\consumer2.txt", msg);

                Thread.Sleep(500);

                _channel.BasicAck(ea.DeliveryTag, false);

                //if (_delegate(obj).GetAwaiter().GetResult())
                //{
                //    channel.BasicAck(ea.DeliveryTag, false);
                //}
                //else
                //{
                //   channel.BasicNack(ea.DeliveryTag, false, true);
                //}

                //Console.WriteLine(" [x] Received {0}", msg);
            };

            _channel.BasicConsume(queue: queue, autoAck: false, consumer: _consumer);

        }
    }
}
