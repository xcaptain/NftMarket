using System.Numerics;
using System.Text.Json;
using Confluent.Kafka;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

Dictionary<string, string> configs =
    new Dictionary<string, string>();
configs.Add("bootstrap.servers", "localhost:19092");
configs["group.id"] = "kafka-dotnet-getting-started2";
configs["auto.offset.reset"] = "earliest";

CancellationTokenSource cts = new CancellationTokenSource();
// Console.CancelKeyPress += (_, e) =>
// {
//     e.Cancel = true; // prevent the process from terminating.
//     cts.Cancel();
// };

using (var consumer = new ConsumerBuilder<string, string>(
            configs).Build())
{
    consumer.Subscribe(new string[] { "raw_blocks", "raw_logs", "raw_txs" });
    try
    {
        while (true)
        {
            var cr = consumer.Consume(cts.Token);
            if (cr.Topic == "raw_logs")
            {
                var l = JsonSerializer.Deserialize<FilterLogVO>(cr.Message.Value);
                if (l.IsLogForEvent<TransferEvent1>())
                {
                    var logEvent = l.Log.DecodeEvent<TransferEvent1>();
                    Console.WriteLine($"event1: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
                }
                else if (l.IsLogForEvent<TransferEvent2>())
                {
                    var logEvent = l.Log.DecodeEvent<TransferEvent2>();
                    Console.WriteLine($"event2: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
                }
                else
                {
                    Console.WriteLine("other event");
                }
            }
            // Console.WriteLine($"Consumed event from topic {cr.Topic.ToString()} with key {cr.Message.Key,-10} and value {cr.Message.Value}");
        }
    }
    finally
    {
        consumer.Close();
    }
}

[Event("Transfer")]
public class TransferEvent1 : IEventDTO
{
    [Parameter("address", "_from", 1, true)]
    public string From { get; set; }

    [Parameter("address", "_to", 2, true)]
    public string To { get; set; }

    [Parameter("uint256", "_value", 3, false)]
    public BigInteger Value { get; set; }
}

[Event("Transfer")]
public class TransferEvent2 : IEventDTO
{
    [Parameter("address", "_from", 1, true)]
    public string From { get; set; }

    [Parameter("address", "_to", 2, true)]
    public string To { get; set; }

    [Parameter("uint256", "_value", 3, true)]
    public BigInteger Value { get; set; }
}