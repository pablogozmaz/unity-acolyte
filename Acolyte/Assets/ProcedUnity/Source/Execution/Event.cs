using System;
using System.Collections;
using System.Collections.Generic;


namespace ProcedUnity
{
    public class Event
    {
        public readonly string name;

        public event Action action;

        private static readonly Dictionary<string, Event> events = new Dictionary<string, Event>();


        public Event(string name)
        {
            this.name = name;

            events.Add(name, this);
        }

        public static bool TryGet(string name, out Event ev) => events.TryGetValue(name, out ev);
        
        public static IEnumerable<string> GetKeys() => events.Keys;

        public void Invoke() 
        {
            action?.Invoke();
        }
    }
}