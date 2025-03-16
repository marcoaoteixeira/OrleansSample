using Microsoft.Extensions.Logging;

namespace Nameless.Orleans.Grains.Filters;

public class LoggingOutgoingGrainCallFilter : IOutgoingGrainCallFilter {
    private readonly ILogger<LoggingOutgoingGrainCallFilter> _logger;

    public LoggingOutgoingGrainCallFilter(ILogger<LoggingOutgoingGrainCallFilter> logger) {
        _logger = logger;
    }

    public Task Invoke(IOutgoingGrainCallContext context) {
        _logger.LogInformation($"Outgoing Silo Filter: Received grain call on '{context.Grain}' to '{context.MethodName}' method");

        return context.Invoke();
    }
}