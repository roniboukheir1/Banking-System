
using BankingSystem.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Application.Common;

namespace BankingSystem.Application.Commands
{
    public class RollbackTransactionsCommand : IRequest
    {
        public DateTime Date { get; set; }
        public int AccountId { get; set; }
    }

    public class RollbackTransactionsCommandHandler : IRequestHandler<RollbackTransactionsCommand>
    {
        private readonly IEventStore _eventStore;

        public RollbackTransactionsCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(RollbackTransactionsCommand request, CancellationToken cancellationToken)
        {
            // Retrieve events for the specified date range
            var events = await _eventStore.GetEventsAsync(request.Date.Date, request.Date.Date.AddDays(1));
            
            events = events.Where(e => e is TransactionCreatedEvent createdEvent && createdEvent.AccountId == request.AccountId);
           

            foreach (var @event in events)
            {
                if (@event is TransactionCreatedEvent createdEvent)
                {
                    var revertEvent = new TransactionRevertedEvent
                    {
                        EventId = createdEvent.EventId,
                        AggregateId = createdEvent.AggregateId,
                        AccountId = createdEvent.AccountId,
                        Amount = createdEvent.Amount,
                        Type = createdEvent.Type,
                        CreatedAt = DateTime.UtcNow // Ensure that we have a timestamp for the revert event
                    };

                    await _eventStore.SaveEventAsync(revertEvent);
                }
            }
        }
    }
}
