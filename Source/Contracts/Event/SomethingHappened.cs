using FastEndpoints;

namespace Contracts;

public sealed class SomethingHappened : IRoundRobinEvent
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string ReceivedBy { get; set; }
}
