using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ProcedUnity
{
    /// <summary>
    /// Contains registered info from an execution. An instance of the nested class Recorder is required to register info.
    /// </summary>
    public sealed class ExecutionRegistry
    {
        public class EntryEvent : UnityEvent<Entry> { }

        /// <summary>
        /// Records info to an execution registry from a specific source.
        /// </summary>
        public class Recorder
        {
            private readonly ExecutionRegistry registry;
            private readonly IExecution source;

            public Recorder(ExecutionRegistry registry, IExecution source)
            {
                Debug.Assert(registry != null);
                this.registry = registry;
                this.source = source;
            }

            public void AddEntry(string key, object content = null) 
            {
                if(registry.IsClosed)
                {
                    Debug.Assert(false);
                    return;
                }

                registry.RegisterEntry(source, key, content);
            }
        }

        public struct Entry
        {
            public IExecution source;
            public DateTime   dateTime;
            public string     key;
            public object     content;
        }

        public static event Action<ExecutionRegistry> OnRegistryCreated;

        public const string KeyProcedureStarted   = "PROCEDURE_STARTED";
        public const string KeyProcedureCompleted = "PROCEDURE_COMPLETED";
        public const string KeyProcedureCancelled = "PROCEDURE_CANCELLED";
        public const string KeyStepStarted        = "STEP_STARTED";
        public const string KeyStepCompleted      = "STEP_COMPLETED";
        public const string KeyStepAutoComplete   = "STEP_AUTOCOMPLETE";
        public const string KeyStepUndo           = "STEP_UNDO";
        public const string KeyActionStarted      = "ACTION_STARTED";
        public const string KeyActionCompleted    = "ACTION_COMPLETED";

        /// <summary>
        /// Invoked when a new entry is added to the register. <para>Automatically turns null when registry is closed.</para>
        /// </summary>
        public EntryEvent OnUpdated { get; private set; } = new EntryEvent();

        /// <summary>
        /// Invoked after the last registered entry involved the closure of the registry.
        /// <para> Automatically turns null when registry is closed. </para>>
        /// </summary>
        public UnityEvent OnClosure { get; private set; } = new UnityEvent();

        /// <summary>
        /// A closed registry wont ever register any more entries.
        /// </summary>
        public bool IsClosed { get; private set; }

        private readonly List<Entry> entries = new List<Entry>();


        public ExecutionRegistry() 
        {
            OnRegistryCreated?.Invoke(this);
        }

        public Entry[] GetAllEntries() 
        {
            return entries.ToArray();
        }

        // Keep private so only Recorder instances can access
        private void RegisterEntry(IExecution source, string key, object content) 
        {
            Debug.Assert(!IsClosed);

            var entry = new Entry()
            {
                source = source,
                dateTime = DateTime.Now,
                key = key,
                content = content
            };

            entries.Add(entry);

            InvokeOnUpdated(entry);

            if(DoesEntryMeanClosure(entry))
            {
                ProcessClosure();
            }
        }

        private void InvokeOnUpdated(Entry entry) 
        {
            try
            {
                OnUpdated.Invoke(entry);
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message + "\n" + ex.StackTrace);
            }
        }

        private bool DoesEntryMeanClosure(Entry entry)
        {
            return entry.key == KeyProcedureCompleted || entry.key == KeyProcedureCancelled;
        }

        private void ProcessClosure()
        {
            try
            {
                OnClosure.Invoke();
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message + "\n" + ex.StackTrace);
            }

            OnClosure.RemoveAllListeners();
            OnUpdated.RemoveAllListeners();
            OnUpdated = null;
            OnClosure = null;
        }
    }
}