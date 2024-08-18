using System.Data.Entity;
using BankingSystem.Domain.Models;
using BankingSystem.Persistance.Data;
using Newtonsoft.Json;
using EntityFrameworkQueryableExtensions = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

namespace BankingSystem.Application.Common;
public interface IEventStore
{
    Task SaveEventAsync(Event @event);
    Task<IEnumerable<Event>> GetEventsAsync(Guid aggregateId);
    Task<IEnumerable<Event>> GetEventsAsync(DateTime from, DateTime to);
}

public class EventStore : IEventStore
{
    private readonly BankingSystemContext _context;

    public EventStore(BankingSystemContext context)
    {
        _context = context;
    }


    public async Task SaveEventAsync(Event @event)
    {
        var eventEntity = new Event
        {
            EventId = @event.EventId,
            AggregateId = @event.AggregateId,
            EventType = @event.GetType().Name, 
            EventData = JsonConvert.SerializeObject(@event), 
            CreatedAt = @event.CreatedAt
        };

        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();
    }


    public async Task<IEnumerable<Event>> GetEventsAsync(Guid aggregateId)
    {
        var eventEntities = await EntityFrameworkQueryableExtensions.Include(_context.Events
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.CreatedAt), @event => @event.EventData)
            .ToListAsync();

        return eventEntities.Select(e => DeserializeEvent(e.EventData, e.EventType));
    }

    public async Task<IEnumerable<Event>> GetEventsAsync(DateTime from, DateTime to)
    {
        var events = await _context.Events
            .Where(e => e.CreatedAt >= from && e.CreatedAt <= to)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();

        return events;
    }

   private Event? DeserializeEvent(string eventData, string eventType)
    {
        return eventType switch
        {
            "TransactionCreated" => JsonConvert.DeserializeObject<TransactionCreatedEvent>(eventData),
            "TransactionReverted" => JsonConvert.DeserializeObject<TransactionRevertedEvent>(eventData),
            _ => throw new NotSupportedException($"Event type '{eventType}' is not supported")
        };
}

}