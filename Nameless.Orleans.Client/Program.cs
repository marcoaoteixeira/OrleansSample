using Azure.Data.Tables;
using Nameless.Orleans.Client.Contracts;
using Nameless.Orleans.Core;
using Nameless.Orleans.Grains.Abstractions;
using Orleans.Configuration;

namespace Nameless.Orleans.Client;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseOrleansClient((_, client) => {
            client.UseAzureStorageClustering(options => {
                options.TableServiceClient = new TableServiceClient(Constants.ConnectionString);
            });

            client.Configure<ClusterOptions>(options => {
                options.ClusterId = Constants.ClusterId;
                options.ServiceId = Constants.ServiceId;
            });

            client.UseTransactions();
        });

        var app = builder.Build();

        // Creates a new ATM
        app.MapPost("/atm", async (CreateAtm createAtm, IClusterClient clusterClient) => {
            var atmId = Guid.NewGuid();
            var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);

            await atmGrain.InitializeAsync(createAtm.OpeningBalance);

            return TypedResults.Created($"/atm/{atmId}", new {
                AtmId = atmId,
                createAtm.OpeningBalance
            });
        });

        // Retrieves the ATM current balance
        app.MapGet("/atm/{atmId}/balance", async (Guid atmId, ITransactionClient transactionClient, IClusterClient clusterClient) => {
            var currentBalance = 0m;
            await transactionClient.RunTransaction(TransactionOption.Create, async () => {
                var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);
                currentBalance = await atmGrain.GetCurrentBalanceAsync();
            });

            return TypedResults.Ok(new {
                AtmId = atmId,
                CurrentBalance = currentBalance
            });
        });

        // Withdraw from ATM
        app.MapPost("/atm/{atmId}/withdraw", async (Guid atmId,
                                                    AtmWithdraw atmWithdraw,
                                                    IClusterClient clusterClient,
                                                    ITransactionClient transactionClient) => {
            var atmGrain = clusterClient.GetGrain<IAtmGrain>(atmId);
            var accountGrain = clusterClient.GetGrain<IAccountGrain>(atmWithdraw.AccountId);

            await transactionClient.RunTransaction(TransactionOption.Create, async () => {
                await atmGrain.WithdrawAsync(atmWithdraw.AccountId, atmWithdraw.Amount);
                await accountGrain.DebitAsync(atmWithdraw.Amount);
            });

            var currentAtmBalance = await atmGrain.GetCurrentBalanceAsync();
            var currentAccountBalance = await accountGrain.GetCurrentBalanceAsync();

            return TypedResults.Ok(new {
                AtmId = atmId,
                AtmCurrentBalance = currentAtmBalance,
                WithdrawAmout = atmWithdraw.Amount,
                atmWithdraw.AccountId,
                AccountCurrentBalance = currentAccountBalance
            });
        });

        // Creates a new Account
        app.MapPost("/account", async (CreateAccount createAccount, ITransactionClient transactionClient, IClusterClient clusterClient) => {
            var accountId = Guid.NewGuid();

            await transactionClient.RunTransaction(TransactionOption.Create, async () => {
                var accountGrain = clusterClient.GetGrain<IAccountGrain>(accountId);

                await accountGrain.InitializeAsync(createAccount.OpeningBalance);
            });

            return TypedResults.Created($"/account/{accountId}", new {
                AccountId = accountId,
                CurrentBalance = createAccount.OpeningBalance
            });
        });

        // Retrieves Account current balance
        app.MapGet("/account/{accountId}/balance", async (Guid accountId, ITransactionClient transactionClient, IClusterClient clusterClient) => {
            var currentBalance = 0m;

            await transactionClient.RunTransaction(TransactionOption.Create, async () => {
                var accountGrain = clusterClient.GetGrain<IAccountGrain>(accountId);
                currentBalance = await accountGrain.GetCurrentBalanceAsync();
            });

            return TypedResults.Ok(new {
                AccountId = accountId,
                CurrentBalance = currentBalance
            });
        });

        // Creates a debit into an Account
        app.MapPost("/account/{accountId}/debit", async (Guid accountId, Debit input, IClusterClient clusterClient) => {
            var accountGrain = clusterClient.GetGrain<IAccountGrain>(accountId);

            await accountGrain.DebitAsync(input.Amount);

            var currentBalance = await accountGrain.GetCurrentBalanceAsync();

            return TypedResults.Ok(new {
                AccountId = accountId,
                Debit = input.Amount,
                CurrentBalance = currentBalance
            });
        });

        // Creates a credit into an Account
        app.MapPost("/account/{accountId}/credit", async (Guid accountId, Credit input, IClusterClient clusterClient) => {
            var accountGrain = clusterClient.GetGrain<IAccountGrain>(accountId);

            await accountGrain.CreditAsync(input.Amount);

            var currentBalance = await accountGrain.GetCurrentBalanceAsync();

            return TypedResults.Ok(new {
                AccountId = accountId,
                Credit = input.Amount,
                CurrentBalance = currentBalance
            });
        });

        // Creates a recurring payment on an Account
        app.MapPost("/account/{accountId}/recurring_payment", async (Guid accountId, RecurringPayment input, IClusterClient clusterClient) => {
            var accountGrain = clusterClient.GetGrain<IAccountGrain>(accountId);

            await accountGrain.AddRecurringPaymentAsync(input.Amount, input.PeriodInMinutes);

            return TypedResults.Ok(new {
                AccountId = accountId,
                RecurringAmout = input.Amount,
                RecurringPeriodInMinutes = input.PeriodInMinutes
            });
        });

        // Get customer net worth
        app.MapGet("/customer/{customerId}/net_worth", async (Guid customerId, IClusterClient clusterClient) => {
            var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

            var netWorth = await customerGrain.GetNetWorthAsync(customerId);

            return TypedResults.Ok(new {
                CustomerId = customerId,
                NetWorth = netWorth
            });
        });

        // Get customer net worth
        app.MapPost("/customer/{customerId:guid}/add_account", async (Guid customerId, CustomerAccount customerAccount, IClusterClient clusterClient) => {
            var customerGrain = clusterClient.GetGrain<ICustomerGrain>(customerId);

            await customerGrain.AddAccountAsync(customerAccount.AccountId);

            return TypedResults.Ok(new {
                CustomerId = customerId,
                customerAccount.AccountId
            });
        });

        app.Run();
    }
}