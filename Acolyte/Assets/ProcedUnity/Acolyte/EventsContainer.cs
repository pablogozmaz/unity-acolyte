using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    public class EventsContainer : Acolyte.IIdentifierContainer<Event>
    {
        public static EventsContainer Instance
        {
            get
            {
                if(instance == null)
                    instance = new EventsContainer();
                return instance;
            }
        }

        private static EventsContainer instance;

        public IEnumerable<string> GetAllIdentifiers() => Event.GetKeys();

        public bool TryGetObject(string identifier, out Event obj) => Event.TryGet(identifier, out obj);
    }
}