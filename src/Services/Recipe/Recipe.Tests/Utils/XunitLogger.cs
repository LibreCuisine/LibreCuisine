using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Recipe.Tests.Utils;

public class XunitLogger<T>: ILogger<T>, IDisposable
{
    private readonly ITestOutputHelper _output;
    public XunitLogger(ITestOutputHelper output)
    {
        _output = output;
    }
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return this;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _output.WriteLine($"{logLevel}: {formatter(state, exception)}");
    }

    public void Dispose()
    {
    }
}