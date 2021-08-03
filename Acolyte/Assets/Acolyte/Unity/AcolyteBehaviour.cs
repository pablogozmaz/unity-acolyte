using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.UnityEngine
{
    public abstract class AcolyteBehaviour : MonoBehaviour
    {
        public static IEnumerable EnabledBehaviours { get { return enabledBehaviours; } }

        private static readonly List<AcolyteBehaviour> enabledBehaviours = new List<AcolyteBehaviour>();

        private static readonly Dictionary<Type, Language> languages = new Dictionary<Type, Language>();

        protected Language Language { get; private set; }


        public void ExecuteScript(Script script)
        {
            script.Execute(new ScriptActionContext());
        }

        protected virtual void Awake()
        {
            Language = GetLanguage();
        }

        protected virtual void OnEnable() 
        {
            enabledBehaviours.Add(this);
        }

        protected virtual void OnDisable()
        {
            enabledBehaviours.Remove(this);
        }

        protected abstract Language CreateLanguage();

        private Language GetLanguage()
        {
            Type type = GetType();

            if(!languages.TryGetValue(type, out Language language))
            {
                language = CreateLanguage();
                languages.Add(type, language);
            }

            return language;
        }
    }
}