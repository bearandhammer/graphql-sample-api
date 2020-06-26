using PizzaOrder.Business.Interfaces;
using PizzaOrder.Business.Models;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PizzaOrder.Business.Services
{
    public class EventService : IEventService
    {
        private readonly ISubject<EventDataModel> onCreateSubject;
        private readonly ISubject<EventDataModel> onStatusUpdateSubject;

        public EventService()
        {
            onCreateSubject = new ReplaySubject<EventDataModel>(1);
            onStatusUpdateSubject = new ReplaySubject<EventDataModel>(1);
        }

        public void AddOrderEvent(EventDataModel orderEvent) => onCreateSubject.OnNext(orderEvent);

        public IObservable<EventDataModel> OnCreateObservable => onCreateSubject.AsObservable();

        public void StatusUpdateEvent(EventDataModel orderEvent) => onStatusUpdateSubject.OnNext(orderEvent);

        public IObservable<EventDataModel> OnStatusUpdateObservable() => onStatusUpdateSubject.AsObservable();
    }
}
