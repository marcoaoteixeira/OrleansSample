using Microsoft.Extensions.Logging;

namespace Nameless.Orleans.Grains.Filters;

public class LoggingIncomingGrainCallFilter : IIncomingGrainCallFilter
{
    private readonly ILogger<LoggingIncomingGrainCallFilter> _logger;

    public LoggingIncomingGrainCallFilter(ILogger<LoggingIncomingGrainCallFilter> logger) {
        _logger = logger;
    }

    public async Task Invoke(IIncomingGrainCallContext context) {
        await context.Invoke();

        _logger.LogInformation($"Incoming Silo Filter: Received grain call on '{context.Grain}' to '{context.MethodName}' method");
    }
}