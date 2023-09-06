using System.Numerics;
using System.Text.Json;
using Confluent.Kafka;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

Dictionary<string, string> configs =
    new()
    {
        { "bootstrap.servers", "localhost:19092" }
    };
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
                if (l.IsLogForEvent<Erc721TransferEvent>())
                {
                    var logEvent = l.Log.DecodeEvent<Erc721TransferEvent>();
                    Console.WriteLine($"Erc721TransferEvent: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
                }
                else if (l.IsLogForEvent<Erc721Erc20LikeTransfer>())
                {
                    var logEvent = l.Log.DecodeEvent<Erc721Erc20LikeTransfer>();
                    Console.WriteLine($"Erc721Erc20LikeTransfer: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
                }
                else if (l.IsLogForEvent<Erc721LikeTransfer>())
                {
                    var logEvent = l.Log.DecodeEvent<Erc721LikeTransfer>();
                    Console.WriteLine($"Erc721LikeTransfer: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
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
public class Erc721TransferEvent : IEventDTO
{
    [Parameter("address", "_from", 1, true)]
    public string From { get; set; }

    [Parameter("address", "_to", 2, true)]
    public string To { get; set; }

    [Parameter("uint256", "_value", 3, true)]
    public BigInteger Value { get; set; }
}



[Event("Transfer")]
public class Erc721LikeTransfer : IEventDTO
{
    [Parameter("address", "_from", 1, false)]
    public string From { get; set; }

    [Parameter("address", "_to", 2, false)]
    public string To { get; set; }

    [Parameter("uint256", "_value", 3, false)]
    public BigInteger Value { get; set; }
}

[Event("Transfer")]
public class Erc721Erc20LikeTransfer : IEventDTO
{
    [Parameter("address", "_from", 1, true)]
    public string From { get; set; }

    [Parameter("address", "_to", 2, true)]
    public string To { get; set; }

    [Parameter("uint256", "_value", 3, true)]
    public BigInteger Value { get; set; }
}