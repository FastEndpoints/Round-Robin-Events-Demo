﻿using Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using PublisherServer;

var bld = WebApplication.CreateBuilder();
bld.WebHost.ConfigureKestrel(o =>
{
    o.ListenLocalhost(6000, o => o.Protocols = HttpProtocols.Http2); // for GRPC
    o.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http1AndHttp2); // for REST
});
bld.AddHandlerServer();
bld.Services.AddSingleton(new DbContext("EventStore", "localhost"));

var app = bld.Build();
app.MapHandlers<PubEventRecord, HubStorageProvider>(h =>
{
    h.RegisterEventHub<SomethingHappened>(HubMode.RoundRobin | HubMode.EventBroker);
});

app.MapGet("/event/{name}", (string name) =>
{
    Parallel.ForEach(Enumerable.Range(1, 10), async i =>
    {
        new SomethingHappened
        {
            Id = i,
            Description = name
        }
        .Broadcast();
    });

    return Results.Ok("events published!");
});

app.Run();

namespace PublisherServer { public partial class Program { }; }
