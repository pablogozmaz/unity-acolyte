using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    public class EventResponseContainer : Acolyte.IIdentifierContainer<EventResponse>
    {
        public static EventResponseContainer Instance
        {
            get
            {
                if(instance == null)
                    instance = new EventResponseContainer();
                return instance;
            }
        }

        private static EventResponseContainer instance;


        public IEnumerable<string> GetAllIdentifiers() => EventResponse.GetAllKeys();

        public bool TryGetObject(string identifier, out EventResponse obj) => EventResponse.TryGet(identifier, out obj);
    }

    public class EventResponse
    {
        public readonly string name;

        private readonly Action action;

        private static readonly Dictionary<string, EventResponse> responses = new Dictionary<string, EventResponse>();


        public EventResponse(string name, Action action)
        {
            this.name = name;
            this.action = action;

            responses.Add(name, this);
        }

        public static bool TryGet(string name, out EventResponse response) => responses.TryGetValue(name, out response);

        public static IEnumerable<string> GetAllKeys() => responses.Keys;

        public void Invoke() 
        {
            action?.Invoke();
        }
    }
}