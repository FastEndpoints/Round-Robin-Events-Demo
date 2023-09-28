using Contracts;
using FastEndpoints;
using SubscriberClient;

var bld = WebApplication.CreateBuilder(args);
bld.Services.AddSingleton(new DbContext("EventStore", "localhost"));
bld.Services.AddEventSubscriberStorageProvider<SubEventRecord, SubscriberStorageProvider>();

var app = bld.Build();

app.MapRemote("http://localhost:6000", c =>
{
    c.Subscribe<SomethingHappened, WhenSomethingHappens>();
});

app.Run();

namespace SubscriberClient { public partial class Program { }; }
