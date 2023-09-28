using Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Concurrent;

namespace Test;

sealed class TestEventHandlerA : IEventHandler<SomethingHappened>
{
    internal static ConcurrentBag<SomethingHappened> Received = new();

    private readonly string subscriber;

    public TestEventHandlerA(IWebHostEnvironment environment)
    {
        subscriber = environment.EnvironmentName;
    }

    public Task HandleAsync(SomethingHappened e, CancellationToken ct)
    {
        e.ReceivedBy = subscriber;
        Received.Add(e);
        return Task.CompletedTask;
    }
}

sealed class TestEventHandlerB : IEventHandler<SomethingHappened>
{
    internal static ConcurrentBag<SomethingHappened> Received = new();

    private readonly string subscriber;

    public TestEventHandlerB(IWebHostEnvironment environment)
    {
        subscriber = environment.EnvironmentName;
    }

    public Task HandleAsync(SomethingHappened e, CancellationToken ct)
    {
        e.ReceivedBy = subscriber;
        Received.Add(e);
        return Task.CompletedTask;
    }
}