using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Nameless.Orleans.Core;
using Nameless.Orleans.Grains.Filters;
using Orleans.Configuration;

namespace Nameless.Orleans.Silo;

public static class Program {
    public static async Task Main(string[] args)
        => await Host.CreateDefaultBuilder()
                     .UseOrleans(server => {
                         server.AddIncomingGrainCallFilter<LoggingIncomingGrainCallFilter>();

                         server.UseAzureStorageClustering(options => {
                             options.TableServiceClient = new TableServiceClient(Constants.ConnectionString);
                         });

                         server.Configure<ClusterOptions>(options => {
                             options.ClusterId = Constants.ClusterId;
                             options.ServiceId = Constants.ServiceId;
                         });

                         //server.Configure<GrainCollectionOptions>(options => {
                         //    options.CollectionQuantum = TimeSpan.FromSeconds(20);
                         //    options.CollectionAge = TimeSpan.FromSeconds(60);
                         //});

                         server.AddAzureTableGrainStorage(StorageNames.TableStorage, options => {
                             options.TableServiceClient = new TableServiceClient(Constants.ConnectionString);
                         });

                         server.AddAzureBlobGrainStorage(StorageNames.BlobStorage, options => {
                             options.BlobServiceClient = new BlobServiceClient(Constants.ConnectionString);
                         });

                         server.UseAzureTableReminderService(configure => {
                             configure.Configure(opts => {
                                 opts.TableServiceClient = new TableServiceClient(Constants.ConnectionString);
                             });
                         });

                         server.AddAzureTableTransactionalStateStorageAsDefault(configure => {
                             configure.Configure(opts => {
                                 opts.TableServiceClient = new TableServiceClient(Constants.ConnectionString);
                             });
                         });

                         server.UseTransactions();

                         //server.AddAzureQueueStreams(StreamNames.StreamProvider, configure => {
                         //    configure.Configure(opts => {
                         //        opts.QueueServiceClient = new QueueServiceClient(Constants.ConnectionString);
                         //    });
                         //}).AddAzureTableGrainStorage(StorageNames.PubSubStore, configure => {
                         //    configure.Configure(opts => {
                         //        opts.TableServiceClient = new TableServiceClient(Constants.ConnectionString);
                         //    });
                         //});
                     })
                     .RunConsoleAsync();
}