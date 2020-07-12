﻿using System;
using System.IO;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ConfigurationService.Client.Subscribers
{
    public class RedisSubscriber : ISubscriber
    {
        private readonly ILogger _logger;
        private static ConnectionMultiplexer _connection;

        public string Name => "Redis";

        public RedisSubscriber(string configuration, ILogger logger)
        {
            _logger = logger;

            using (var writer = new StringWriter())
            {
                _connection = ConnectionMultiplexer.Connect(configuration, writer);

                _logger.LogDebug(writer.ToString());
            }

            _connection.ErrorMessage += (sender, args) => { _logger.LogError(args.Message); };

            _connection.ConnectionFailed += (sender, args) => { _logger.LogError(args.Exception, "Redis connection failed."); };

            _connection.ConnectionRestored += (sender, args) => { _logger.LogInformation("Redis connection restored."); };
        }

        public void Subscribe(string topic, Action<object> handler)
        {
            _logger.LogInformation("Subscribing to Redis channel '{topic}'.", topic);

            var subscriber = _connection.GetSubscriber();

            subscriber.Subscribe(topic, (channel, message) =>
            {
                _logger.LogInformation("Received subscription on Redis channel '{channel}'.", channel);

                handler(message);
            });

            var endpoint = subscriber.SubscribedEndpoint(topic);
            _logger.LogInformation("Subscribed to Redis endpoint {endpoint} for channel '{topic}'.", endpoint, topic);
        }
    }
}