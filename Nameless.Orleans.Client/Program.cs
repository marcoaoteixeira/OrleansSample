using Azure.Data.Tables;
using Nameless.Orleans.Client.Contracts;
using Nameless.Orleans.Grains.Abstractions;
using Orleans.Configuration;

namespace Nameless.Orleans.Client;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseOrleansClient((_, client) => {
            client.UseAzureStorageClustering(configureOptions: options => {
                options.TableServiceClient = new TableServiceClient("UseDevelopmentStorage=true;");
            });

            client.Configure<ClusterOptions>(options => {
                options.ClusterId = "Nameless_Orleans_Cluster";
                options.ServiceId = "Nameless_Orleans_Service";
            });
        });

        var app = builder.Build();

        app.MapGet("/account/{accountId}/balance", async (Guid accountId, IClusterClient clusterClient) => {
            var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(accountId);
            var balance = await checkingAccountGrain.GetBalance();

            return TypedResults.Ok(balance);
        });

        app.MapPost("/account", async (CreateAccount createAccount, IClusterClient clusterClient) => {
            var accountId = Guid.NewGuid();
            var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(accountId);

            await checkingAccountGrain.InitializeAsync(createAccount.OpeningBalance);

            return TypedResults.Created($"/account/{accountId}");
        });

        app.Run();
    }
}