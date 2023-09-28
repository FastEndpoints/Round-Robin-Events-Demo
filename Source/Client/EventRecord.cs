﻿#pragma warning disable CS8618
using FastEndpoints;
using MongoDB.Entities;

namespace SubscriberClient;

public class SubEventRecord : Entity, IEventStorageRecord
{
    public string SubscriberID { get; set; }
    public object Event { get; set; }
    public string EventType { get; set; }
    public DateTime ExpireOn { get; set; }
    public bool IsComplete { get; set; }
}