using PizzaOrder.Business.Models;
using System;

namespace PizzaOrder.Business.Interfaces
{
    public interface IEventService
    {
        IObservable<EventDataModel> OnCreateObservable { get; }

        void AddOrderEvent(EventDataModel orderEvent);

        IObservable<EventDataModel> OnStatusUpdateObservable();

        void StatusUpdateEvent(EventDataModel orderEvent);
    }
}
