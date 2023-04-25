using Nethereum.Web3;
using Confluent.Kafka;

// var web3 = new Web3("https://sepolia.infura.io/v3/886f9a612855454696499897ae97d905");

// bootstrap.servers=localhost:9092
Dictionary<string, string> configs =
    new Dictionary<string, string>();
configs.Add("bootstrap.servers", "localhost:19092");

const string topic = "purchases";

string[] users = { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
string[] items = { "book", "alarm clock", "t-shirts", "gift card", "batteries" };


using (var producer = new ProducerBuilder<string, string>(
            configs).Build())
{
    var numProduced = 0;
    Random rnd = new Random();
    const int numMessages = 10;
    for (int i = 0; i < numMessages; ++i)
    {
        var user = users[rnd.Next(users.Length)];
        var item = items[rnd.Next(items.Length)];

        producer.Produce(topic, new Message<string, string> { Key = user, Value = item },
            (deliveryReport) =>
            {
                if (deliveryReport.Error.Code != ErrorCode.NoError)
                {
                    Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                }
                else
                {
                    Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                    numProduced += 1;
                }
            });
    }

    producer.Flush(TimeSpan.FromSeconds(10));
    Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
}

// var client = new EthClient(web3);
// await client.CrawlBlocks();