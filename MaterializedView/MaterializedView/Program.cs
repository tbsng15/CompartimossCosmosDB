﻿using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.ChangeFeedProcessor;

namespace LeaderboardChangeFeed

{
    class LeaderboardChangeFeedProcessor
    {
        static void Main(string[] args)
        {
            var p = new LeaderboardChangeFeedProcessor();
            p.Run();
        }
        public void Run()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            DocumentCollectionInfo feedCollectionInfo = new DocumentCollectionInfo()
            {
                DatabaseName = "pinball-leaderboard",
                CollectionName ="leaderboard",
                Uri = new Uri("https://sqlcomos2019.documents.azure.com:443/"),
                MasterKey = "0oyBJST0UzbkkU2hIYMmePZAEjBoTPTW7EEqUCL7WPhxNN2DNai8TwpKfnQrE6mlN4R93PGe08fGIoK2FtAYfw=="
            };

            DocumentCollectionInfo leaseCollectionInfo = new DocumentCollectionInfo()
            {
                DatabaseName = "pinball-leaderboard",
                CollectionName = "leasesScore",
                Uri = new Uri("https://sqlcomos2019.documents.azure.com:443/"),
                MasterKey = "0oyBJST0UzbkkU2hIYMmePZAEjBoTPTW7EEqUCL7WPhxNN2DNai8TwpKfnQrE6mlN4R93PGe08fGIoK2FtAYfw=="
            };

            ChangeFeedProcessorOptions feedProcessorOptions = new ChangeFeedProcessorOptions();

            feedProcessorOptions.LeaseRenewInterval = TimeSpan.FromSeconds(5);
            feedProcessorOptions.StartFromBeginning = false;

            var builder = new ChangeFeedProcessorBuilder();
            var processor = await builder
                .WithHostName("LeaderboardHost")
                .WithFeedCollection(feedCollectionInfo)
                .WithLeaseCollection(leaseCollectionInfo)
                .WithObserver<ChangeFeedObserver>()
                .WithProcessorOptions(feedProcessorOptions)
                .BuildAsync();

            await processor.StartAsync();

            Console.WriteLine("Change Feed Processor started. Press <Enter> key to stop...");

            Console.ReadLine();

            await processor.StopAsync();
        }
    }
}