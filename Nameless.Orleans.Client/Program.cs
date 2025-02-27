using Azure.Data.Tables;
using Nameless.Orleans.Client.Contracts;
using Nameless.Orleans.Grains.Abstractions;
using Orleans.Configuration;

namespace Nameless.Orleans.Client;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseOrleansClient((_, client) => {
            client.UseAzureStorageClustering(options => {
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
            var balance = await checkingAccountGrain.GetBalanceAsync();

            return TypedResults.Ok(balance);
        });

        app.MapPost("/account", async (CreateAccount createAccount, IClusterClient clusterClient) => {
            var accountId = Guid.NewGuid();
            var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(accountId);

            await checkingAccountGrain.InitializeAsync(createAccount.OpeningBalance);

            return TypedResults.Created($"/account/{accountId}", new { AccountId = accountId });
        });

        app.MapPost("/account/{accountId}/debit", async (Guid accountId, Debit input, IClusterClient clusterClient) => {
            var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(accountId);

            await checkingAccountGrain.DebitAsync(input.Amount);

            return TypedResults.Ok(input);
        });

        app.MapPost("/account/{accountId}/credit", async (Guid accountId, Credit input, IClusterClient clusterClient) => {
            var checkingAccountGrain = clusterClient.GetGrain<ICheckingAccountGrain>(accountId);

            await checkingAccountGrain.CreditAsync(input.Amount);

            return TypedResults.Ok(input);
        });

        app.MapPost("/atm", async (CreateAtm createAtm, IClusterClient clusterClient) => {
            var atmId = Guid.NewGuid();
            var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);

            await atmGrain.InitializeAsync(createAtm.OpeningBalance);

            return TypedResults.Created($"/atm/{atmId}", new { AtmId = atmId });
        });

        app.MapPost("/atm/{atmId}/withdraw", async (Guid atmId, AtmWithdraw atmWithdraw, IClusterClient clusterClient) => {
            var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);

            await atmGrain.WithdrawAsync(atmWithdraw.AccountId, atmWithdraw.Amount);

            return TypedResults.Ok(atmWithdraw);
        });

        app.Run();
    }
}