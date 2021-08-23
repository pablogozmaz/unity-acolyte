using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public interface IExpression {}

    public class Expression<T> : IExpression where T : struct
    {
        private readonly Dictionary<string, Func<T>> dictionary = new Dictionary<string, Func<T>>();

        public void Add(string name, Func<T> value)
        {
            dictionary.Add(name, value);
        }

        public bool TryGet(string name, out T value)
        {
            if(dictionary.TryGetValue(name, out Func<T> func))
            {
                value = func.Invoke();
                return true;
            }

            value = default;
            return false;
        }
    }
}