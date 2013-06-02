using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orbit.Core
{
    /// <summary>
    /// umoznuje planovani a automaticke provadeni vlozenych akci
    /// </summary>
    public class EventProcessor : IUpdatable
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDictionary<int, Event> events;
        private IList<int> eventsForRemoval;
        private IDictionary<int, Event> eventsForAdding;

        public EventProcessor()
        {
            events = new Dictionary<int, Event>();
            eventsForRemoval = new List<int>();
            eventsForAdding = new Dictionary<int, Event>();
        }

        /// <summary>
        /// prida novy event do fronty provadeni
        /// </summary>
        /// <param name="id">id pod kterym event vystupuje - je unikatni a lze podle jen event zrusit nebo preplanovat</param>
        /// <param name="newEvent">novy event</param>
        public void AddEvent(int id, Event newEvent)
        {
            if (eventsForAdding.ContainsKey(id))
                Logger.Warn("EventProcessor already contains event with id " + id);
            else
                eventsForAdding.Add(id, newEvent);
        }

        /// <summary>
        /// preplanovani eventu - event se nastavi novy timer a zacne od zacatku
        /// </summary>
        /// <param name="id">id eventu ke preplanovani</param>
        /// <param name="newTimer">nova hodnota timeru (v sekundach)</param>
        public void RescheduleEvent(int id, float newTimer)
        {
            if (!events.ContainsKey(id))
                Logger.Warn("EventProcessor is trying to reschedule event id " + id + " but it doesn't exist");
            else
            {
                events[id].Timer = newTimer;
                events[id].OriginalTimer = newTimer;
            }
        }

        /// <summary>
        /// odstrani event z fronty provadeni
        /// </summary>
        /// <param name="id">id eventu, ktery ma byt odstranen</param>
        public void RemoveEvent(int id)
        {
            if (!events.ContainsKey(id))
                Logger.Warn("EventProcessor is trying to remove event id " + id + " but it doesn't exist");
            else
                eventsForRemoval.Add(id);
        }

        /// <summary>
        /// Update je u controlu volan automaticky, jinde ho je treba rucne volat v kazdem updatu
        /// </summary>
        /// <param name="tpf">time per frame</param>
        public void Update(float tpf)
        {
            if (eventsForAdding.Count == 0 && events.Count == 0)
                return;

            foreach (KeyValuePair<int, Event> e in events)
            {
                e.Value.Timer -= tpf;
                if (e.Value.Timer <= 0)
                {
                    e.Value.EventAction.Invoke();

                    if (e.Value.Type == EventType.REPEATABLE)
                        RescheduleEvent(e.Key, e.Value.OriginalTimer);
                    else if (e.Value.Type == EventType.ONE_TIME)
                        RemoveEvent(e.Key);
                }
            }

            foreach (int id in eventsForRemoval)
                events.Remove(id);
            eventsForRemoval.Clear();

            foreach (int id in eventsForAdding.Keys)
            {
                if (events.ContainsKey(id))
                    Logger.Warn("EventProcessor already contains event with id " + id + " during direct adding");
                else
                    events.Add(id, eventsForAdding[id]);
            }
            eventsForAdding.Clear();
        }
    }

    public class Event
    {
        private Action action;
        /// <summary>
        /// akce, ktera se ma provest pri spusteni eventu
        /// </summary>
        public Action EventAction { get { return action; } }

        /// <summary>
        /// rucne nenastavovat, pouziva se u repeatable eventu
        /// </summary>
        public float OriginalTimer { get; set; }

        /// <summary>
        /// timer je pocet sekund, po kterem je spusten event
        /// </summary>
        public float Timer { get; set; }

        private EventType type;
        /// <summary>
        /// typ eventu urcuje jeho chovani - jestli se ma po spusteni znovu naplanovat nebo zkoncit
        /// </summary>
        public EventType Type { get { return type; } }

        /// <summary>
        /// novy event, ktery se vykona po danem case
        /// </summary>
        /// <param name="timer">cas v sekundach, po kterem se akce vykona</param>
        /// <param name="eventType">typ eventu - jestli se ma po spusteni znovu naplanovat nebo zkoncit</param>
        /// <param name="eventAction">event, ktery se provede pri spusteni</param>
        public Event(float timer, EventType eventType, Action eventAction)
        {
            Timer = timer;
            OriginalTimer = timer;
            type = eventType;
            action = eventAction;
        }
    }

    public enum EventType
    {
        ONE_TIME,
        REPEATABLE
    }
}
