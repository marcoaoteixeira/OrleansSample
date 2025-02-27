using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace Nameless.Orleans.Silo;

public static class Program {
    public static async Task Main(string[] args) =>
        await Host.CreateApplicationBuilder()
                  .UseOrleans(server => {
                      server.UseAzureStorageClustering(options => {
                          options.TableServiceClient = new TableServiceClient("UseDevelopmentStorage=true;");
                      });

                      server.Configure<ClusterOptions>(options => {
                          options.ClusterId = "Nameless_Orleans_Cluster";
                          options.ServiceId = "Nameless_Orleans_Service";
                      });

                      server.Configure<GrainCollectionOptions>(options => {
                          options.CollectionQuantum = TimeSpan.FromSeconds(20);
                          options.CollectionAge = TimeSpan.FromSeconds(60);
                      });

                      server.AddAzureTableGrainStorage("tableStorage", options => {
                          options.TableServiceClient = new TableServiceClient("UseDevelopmentStorage=true;");
                      });

                      server.AddAzureBlobGrainStorage("blobStorage", options => {
                          options.BlobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true;");
                      });

                      //// The default
                      //server.AddAzureTableGrainStorageAsDefault(options => {
                      //    options.TableServiceClient = new TableServiceClient("UseDevelopmentStorage=true;");
                      //});
                  })
                  .Build()
                  .RunAsync();
}