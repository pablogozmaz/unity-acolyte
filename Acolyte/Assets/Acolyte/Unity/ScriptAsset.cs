using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte
{
    public abstract class ScriptAsset : ScriptableObject
    {
        public abstract Script Script { get; }

        [SerializeField, Multiline]
        protected string source;

        protected Script script;
    }

    public abstract class ScriptAsset<T> : ScriptAsset where T : Language, new()
    {
        public override Script Script
        {
            get
            {
                if(script == null)
                {
                    if(!languages.TryGetValue(typeof(T), out Language lang))
                    {
                        lang = new T();
                        languages.Add(typeof(T), lang);
                    }

                    script = new Script(source, lang, autoCompileOnInitialization);
                }

                return script;
            }
        }

        private static readonly Dictionary<Type, Language> languages = new Dictionary<Type, Language>();

        protected abstract bool autoCompileOnInitialization { get; }
    }
}