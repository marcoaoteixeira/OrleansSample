using Azure.Data.Tables;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Nameless.Orleans.Silo;

public static class Program {
    public static async Task Main(string[] args) {
        await Host.CreateApplicationBuilder()
                  .UseOrleans(server => {
                      server.UseAzureStorageClustering(configureOptions: options => {
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
                  })
                  .Build()
                  .RunAsync();
    }
}