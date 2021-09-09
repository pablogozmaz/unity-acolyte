using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    /// <summary>
    /// Interface inherited by all expression types.
    /// </summary>
    public interface IExpressions
    {
        IEnumerable<string> GetAllExpressionTokens();

        bool Contains(string name);
    }

    public class Expressions<T> : IExpressions where T : struct
    {
        private readonly Dictionary<string, Func<T>> dictionary = new Dictionary<string, Func<T>>();


        public void Add(string name, Func<T> value)
        {
            dictionary.Add(name, value);
        }

        public bool TryGet(string name, out T value)
        {
            if(dictionary.TryGetValue(name, out Func<T> func))
            {                value = func.Invoke();
                return true;
            }

            value = default;
            return false;
        }

        public bool Contains(string name)
        {
            return dictionary.ContainsKey(name);
        }

        public IEnumerable<string> GetAllExpressionTokens()
        {
            return dictionary.Keys;
        }
    }
}