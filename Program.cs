using Nethereum.Web3;

var web3 = new Web3("https://sepolia.infura.io/v3/886f9a612855454696499897ae97d905");
var client = new EthClient(web3);
await client.CrawlBlocks();