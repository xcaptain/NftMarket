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

        var repoFactory = new CsvBlockchainStoreRepositoryFactory("csvdata");

        //create our processor
        // var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
        // {
        //     // inject handler for each step
        //     steps.BlockStep.AddSynchronousProcessorHandler(b => blocks.Add(b));
        //     steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
        //     steps.ContractCreationStep.AddSynchronousProcessorHandler(cc => contractCreations.Add(cc));
        //     steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
        // });

        var processor = this.web3.Processing.Blocks.CreateBlockStorageProcessor(repoFactory);

        //if we need to stop the processor mid execution - call cancel on the token
        var cancellationToken = new CancellationToken();

        //crawl the required block range
        await processor.ExecuteAsync(
          toBlockNumber: new BigInteger(3269520),
          cancellationToken: cancellationToken,
          startAtBlockNumberIfNotProcessed: new BigInteger(3269517));

        Console.WriteLine($"Blocks Found: {blocks.Count}");
        Console.WriteLine($"Transactions Found: {transactions.Count}");
        Console.WriteLine($"Contract Creations Found: {contractCreations.Count}");

    }
}