﻿using Confluent.Kafka;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Confluent.Kafka.ConfigPropertyNames;

namespace consumer
{
    internal class consumerService : BackgroundService, IConsumerService
    {
        private readonly IConsumer<int, CloudEvent> consumer;
        private readonly string topicName;
        private readonly TelemetryClient _telemetryClient;

        public consumerService(ConsumerConfig config, string topic, TelemetryClient telemetryClient)
        {
            consumer = new ConsumerBuilder<int, CloudEvent>(config)
                .SetValueDeserializer(new CloudEvent()).Build();
            topicName = topic;
            _telemetryClient = telemetryClient;
        }
        public async Task Receive()
        {
            await ExecuteAsync(CancellationToken.None);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                consumer.Subscribe(topicName);
                try
                {
                    Console.WriteLine($"Trying to consume events on topic '{topicName}'...");
                    var result = consumer.Consume(stoppingToken);
                    Offset originalOffset = consumer.Position(result.TopicPartition);
                    try
                    {
                        using (var localTransaction = new TransactionScope())
                        {
                            consumer.StoreOffset(result);
                            //Save internal progress here
                            await new MessageReceivedEventHandler().Handle(result, _telemetryClient);

                            // Only risk of uncoordinated failure is a crash between these two lines...
                            consumer.Commit(result);
                            localTransaction.Complete();
                        }
                    }
                    catch (TransactionAbortedException)
                    {
                        //Restore the offset position
                        consumer.StoreOffset(new TopicPartitionOffset(result.TopicPartition, originalOffset));
                        consumer.Commit(result);
                    }
                }
                catch (Exception ex)
                {
                    _telemetryClient.TrackException(ex);
                    Console.WriteLine($"Failed to consume events on topic '{topicName}': {ex.Message}");
                    Thread.Sleep(10000);
                }
            }
        }
    }

    internal interface IConsumerService
    {
        Task Receive();
    }

    internal class CloudEvent : IDeserializer<CloudEvent>
    {
        public string? Id { get; set; }
        public string? OperationId { get; set; }
        public string? OperationParentId { get; set; }
        public string? Message { get; set; }

        public CloudEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var messageParts = Encoding.UTF8.GetString(data).Split("|");
            return new CloudEvent()
            {
                Id = messageParts[0],
                OperationId = messageParts[1],
                OperationParentId = messageParts[2],
                Message = messageParts[3]
            };
        }
    }

}
