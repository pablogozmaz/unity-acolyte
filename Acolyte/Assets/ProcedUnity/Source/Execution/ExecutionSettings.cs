using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    /// <summary>
    /// Contains settings data interpreted by the executions to determine their behaviour.
    /// <para>
    /// Custom data is supported for execution sub-classes through a string-string dictionary.
    /// </para>
    /// </summary>
    public struct ExecutionSettings
    {
        public bool canAutoComplete;
        public bool canStepBackwards;

        private Dictionary<string, string> customSettings;


        public static ExecutionSettings DefaultSettings
        {
            get 
            {
                return new ExecutionSettings()
                {
                    canStepBackwards = true,
                    canAutoComplete = true,
                };
            }
        }

        public bool AddCustomSetting(string name, string value, bool overwrite = true)
        {
            if(customSettings == null) customSettings = new Dictionary<string, string>();

            if(customSettings.ContainsKey(name))
            {
                if(overwrite)
                    customSettings[name] = value;
                else
                {
                    Debug.Assert(false, "Attempted to add a custom setting without overwriting but it is already present: " + name);
                    return false;
                }
            }
            else
                customSettings.Add(name, value);

            return true;
        }

        public bool TryGetCustomSetting(string name, out string value)
        {
            if(customSettings == null)
            {
                value = null;
                return false;
            }

            return customSettings.TryGetValue(name, out value);
        }
    }
}