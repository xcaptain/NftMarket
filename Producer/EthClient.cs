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
using Nethereum.BlockchainStore.EFCore.Sqlite;
using Nethereum.BlockchainStore.EFCore.Repositories;
using Nethereum.BlockchainStore.EFCore;
using Confluent.Kafka;

public class EthClient
{
    private IWeb3 web3;

    public EthClient(IWeb3 web3)
    {
        this.web3 = web3;
    }

    public async Task CrawlBlocks(IProducer<string, string> producer)
    {
        // var blocks = new List<BlockWithTransactions>();
        // var transactions = new List<TransactionReceiptVO>();
        // var contractCreations = new List<ContractCreationVO>();
        // var filterLogs = new List<FilterLogVO>();

        var context = new SqliteBlockchainDbContextFactory($"Data Source=nft_market.db");
        var repoFactory = new BlockchainStoreRepositoryFactory(context);

        // var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
        // {
        //     steps.BlockStep.AddSynchronousProcessorHandler(b => blocks.Add(b));
        //     steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
        //     steps.ContractCreationStep.AddSynchronousProcessorHandler(cc => contractCreations.Add(cc));
        //     steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
        // });

        var processor = this.web3.Processing.Blocks.CreateBlockStorageProcessor(repoFactory, configureSteps: steps =>
        {
            steps.BlockStep.AddSynchronousProcessorHandler(b =>
            {
                Console.WriteLine("block: " + b.Number.Value.ToString());
                producer.Produce("raw_blocks", new Message<string, string> { Key = b.Number.Value.ToString(), Value = JsonSerializer.Serialize(b) });
            });
            steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx =>
            {
                Console.WriteLine($"tx: {tx.TransactionHash}");
                producer.Produce("raw_txs", new Message<string, string> { Key = tx.TransactionHash.ToString(), Value = JsonSerializer.Serialize(tx) });
            });
            steps.FilterLogStep.AddSynchronousProcessorHandler(l => producer.Produce("raw_logs", new Message<string, string> { Key = l.Log.TransactionHash.ToString(), Value = JsonSerializer.Serialize(l) }));
        });

        //if we need to stop the processor mid execution - call cancel on the token
        var cancellationToken = new CancellationToken();

        //crawl the required block range
        // await processor.ExecuteAsync(
        //   toBlockNumber: new BigInteger(3269520),
        //   cancellationToken: cancellationToken,
        //   startAtBlockNumberIfNotProcessed: new BigInteger(3269520));
        await processor.ExecuteAsync(startAtBlockNumberIfNotProcessed: 2800000, cancellationToken: cancellationToken);

        // Console.WriteLine($"Blocks Found: {blocks.Count}");
        // Console.WriteLine($"Transactions Found: {transactions.Count}");
        // Console.WriteLine($"Contract Creations Found: {contractCreations.Count}");

        // foreach (var l in filterLogs)
        // {
        //     if (l.IsLogForEvent<TransferEvent1>())
        //     {
        //         var logEvent = l.Log.DecodeEvent<TransferEvent1>();
        //         Console.WriteLine($"event1: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
        //     }
        //     else if (l.IsLogForEvent<TransferEvent2>())
        //     {
        //         var logEvent = l.Log.DecodeEvent<TransferEvent2>();
        //         Console.WriteLine($"event2: from: {logEvent.Event.From}, to: {logEvent.Event.To}, value: {logEvent.Event.Value}");
        //     }
        //     else
        //     {
        //         Console.WriteLine("other event");
        //     }
        // }

    }
}

