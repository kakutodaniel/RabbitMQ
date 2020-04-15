﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Producer._1
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

            var queue = "catalog";
            var exchange = "vtex-seller";

            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //channel.ConfirmSelect();

                    var json = JsonConvert.SerializeObject(new { id = "123", seller = "9879" });

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.ContentType = "application/json";
                    properties.DeliveryMode = 2;

                    channel.ExchangeDeclare(exchange: exchange, type: "direct");

                    var _args = new Dictionary<string, object>();
                    //_args.Add("x-message-ttl", 60000);   //expire message
                    //480000 (8hs)
                    _args.Add("x-expires", 6000000);   //expire queue if not activate miliseconds (15 days is enough)
                    //_args.Add("x-queue-mode", "lazy");   //lazy mode, store message on disk

                    channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: _args);

                    channel.QueueBind(queue: queue, exchange: exchange, routingKey: "vtex.seller.catalog.2", arguments: null);

                    channel.BasicPublish(exchange: exchange, routingKey: "vtex.seller.catalog.2", basicProperties: properties, body: ConvertToByte(json));

                    //channel.WaitForConfirmsOrDie(TimeSpan.FromMilliseconds(1));


                    //Parallel.For(0, 10, x =>
                    //{
                    //    var json = JsonConvert.SerializeObject(new { id = x, seller = "9879" });

                    //    var properties = channel.CreateBasicProperties();
                    //    properties.Persistent = true;
                    //    properties.ContentType = "application/json";
                    //    properties.DeliveryMode = 2;

                    //    channel.ExchangeDeclare(exchange: queue, type: "direct");

                    //    var _args = new Dictionary<string, object>();
                    //    _args.Add("x-message-ttl", 60000);
                    //    //_args.Add("x-expires", 60000);
                    //    //_args.Add("x-queue-mode", "lazy");

                    //    channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: _args);

                    //    channel.QueueBind(queue: queue, exchange: queue, routingKey: string.Empty, arguments: null);

                    //    channel.ConfirmSelect();

                    //    channel.BasicPublish(exchange: queue, routingKey: string.Empty, basicProperties: properties, body: ConvertToByte(json));

                    //    channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));

                    //});


                }
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private static byte[] ConvertToByte(string obj)
        {
            return Encoding.UTF8.GetBytes(obj);
        }
    }
}