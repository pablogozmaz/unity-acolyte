using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acolyte
{
    /// <summary>
    /// Abstract scriptable object that should always have a single instance that is automatically loaded from Resources.
    /// </summary>
    public abstract class StaticScriptableObject<T> : ScriptableObject where T : StaticScriptableObject<T>
    {
        protected static T Instance
        {
            get
            {
                if(instance == null)
                {
                    SetInstance();
                }

                return instance;
            }
        }

        protected static void SetInstance()
        {
            var array = Resources.LoadAll<T>("");
            if(array.Length == 0)
            {
                Debug.LogError("<color=red>" + typeof(T) + " has been accessed, but there was an error:</color>");
                Debug.LogError("<color=red>A static scriptable object should be present in the Resources folder.</color>");
            }
            else
            {
                instance = array[0];
            }
        }

        private static T instance;
    }
}