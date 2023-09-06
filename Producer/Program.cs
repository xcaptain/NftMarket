using Nethereum.Web3;
using Confluent.Kafka;
using Producer;

var web3 = new Web3("https://rpc.ankr.com/eth");
var client = new EthClient(web3);

Dictionary<string, string> configs =
    new()
    {
        { "bootstrap.servers", "localhost:19092" }
    };


using (var producer = new ProducerBuilder<string, string>(
            configs).Build())
{
    await client.CrawlBlocks(producer);
    producer.Flush(TimeSpan.FromSeconds(10));
}

// await client.ParseEvents(18063043);