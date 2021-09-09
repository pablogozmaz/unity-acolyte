using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte
{
    public interface IUnityContainerBehaviour
    {
        IEnumerable<string> GetAllIdentifiers();
    }

    /// <summary>
    /// Serializes objects identified with a string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityContainerBehaviour<T> : MonoBehaviour, IUnityContainerBehaviour where T : class
    {
        [Serializable]
        private struct Data
        {
            public string identifier;
            public T obj;
        }

        [SerializeField]
        private Data[] data;

        private readonly Dictionary<string, T> dictionary = new Dictionary<string, T>();


        public bool TryGetObject(string identifier, out T obj)
        {
            if(dictionary.TryGetValue(identifier, out T objects))
            {
                obj = objects;
                return true;
            }

            obj = null;
            return false;
        }

        public IEnumerable<string> GetAllIdentifiers() 
        {
            return dictionary.Keys;
        }

        private void Awake()
        {
            foreach(var d in data)
            {
                dictionary.Add(d.identifier, d.obj);
            }
        }
    }

}