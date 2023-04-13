using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.CQS;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using System.Text.Json;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.BlockchainStore.Csv;

public class EthClient
{
    private IWeb3 web3;

    public EthClient(IWeb3 web3)
    {
        this.web3 = web3;
    }

    public async Task CrawlBlocks()
    {
        var blocks = new List<BlockWithTransactions>();
        var transactions = new List<TransactionReceiptVO>();
        var contractCreations = new List<ContractCreationVO>();
        var filterLogs = new List<FilterLogVO>();

        var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
        {
            steps.BlockStep.AddSynchronousProcessorHandler(b => blocks.Add(b));
            steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
            steps.ContractCreationStep.AddSynchronousProcessorHandler(cc => contractCreations.Add(cc));
            steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
        });

        // var processor = this.web3.Processing.Blocks.CreateBlockStorageProcessor(repoFactory);

        //if we need to stop the processor mid execution - call cancel on the token
        var cancellationToken = new CancellationToken();

        //crawl the required block range
        await processor.ExecuteAsync(
          toBlockNumber: new BigInteger(3269520),
          cancellationToken: cancellationToken,
          startAtBlockNumberIfNotProcessed: new BigInteger(3269520));

        Console.WriteLine($"Blocks Found: {blocks.Count}");
        Console.WriteLine($"Transactions Found: {transactions.Count}");
        Console.WriteLine($"Contract Creations Found: {contractCreations.Count}");

        foreach(var l in filterLogs) {
            if (l.IsLogForEvent<TransferEvent1>()) {
                var logEvent = l.Log.DecodeEvent<TransferEvent1>();
                Console.WriteLine($"event1: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
            } else if (l.IsLogForEvent<TransferEvent2>()) {
                var logEvent = l.Log.DecodeEvent<TransferEvent2>();
                Console.WriteLine($"event2: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
            } else {
                Console.WriteLine("other event");
            }
        }

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