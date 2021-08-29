using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte
{
    public abstract class ScriptAsset : ScriptableObject
    {
        public abstract Script Script { get; }

        protected abstract bool AutoCompileOnInitialization { get; }

        [SerializeField, Multiline]
        protected string source;

        protected Script script;

        public void Modify(string newSourceCode)
        {
            source = newSourceCode;
            script.Modify(newSourceCode, AutoCompileOnInitialization);
        }
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

                    script = new Script(source, lang, AutoCompileOnInitialization);
                }

                return script;
            }
        }

        private static readonly Dictionary<Type, Language> languages = new Dictionary<Type, Language>();
    }
}