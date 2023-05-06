using Nethereum.Web3;
using Confluent.Kafka;

var web3 = new Web3("https://sepolia.infura.io/v3/886f9a612855454696499897ae97d905");
var client = new EthClient(web3);

Dictionary<string, string> configs =
    new Dictionary<string, string>();
configs.Add("bootstrap.servers", "localhost:19092");


using (var producer = new ProducerBuilder<string, string>(
            configs).Build())
{
    await client.CrawlBlocks(producer);
    producer.Flush(TimeSpan.FromSeconds(10));
}