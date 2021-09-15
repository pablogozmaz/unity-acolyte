using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace ProcedUnity.DebugUI
{
    /// <summary>
    /// Logs to UI all execution registry entries.
    /// </summary>
    public class ExecutionRegistryView : MonoBehaviour
    {
        private readonly StringBuilder log = new StringBuilder();

        [SerializeField]
        private TextMeshProUGUI logField;

        [SerializeField]
        private ScrollRect scroll;


        private void Awake()
        {
            logField.text = "";

            ExecutionRegistry.OnRegistryCreated += StartListeningToNewRegistry;
        }

        private void OnDestroy()
        {
            ExecutionRegistry.OnRegistryCreated -= StartListeningToNewRegistry;
        }

        private void StartListeningToNewRegistry(ExecutionRegistry registry) 
        {
            registry.OnUpdated.AddListener(PrintEntry);
        }

        private void PrintEntry(ExecutionRegistry.Entry entry) 
        {
            log.Append("<b>></b> ").Append(entry.dateTime.ToLongTimeString());
            log.Append(" ").Append(GetExecutionPrefix(entry.source));
            log.Append(" <b>").Append(entry.source.Label).Append("</b> - ");
            log.Append(ProcessKey(entry.key));

            if(entry.content != null && !string.IsNullOrEmpty(entry.content.ToString()))
            {
                log.Append(" :: "+entry.content);
            }
            log.Append("\n");

            logField.text = log.ToString();

            LayoutRebuilder.ForceRebuildLayoutImmediate(logField.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);

            scroll.verticalNormalizedPosition = 0;
        }

        private string GetExecutionPrefix(IExecution execution) 
        {
            if(execution is ProcedureExecution)
                return "(P)";
            else 
            if(execution is StepExecution)
                return "(S)";
            else
                return "(A)";
        }

        private string ProcessKey(string key)
        {
            if(key.EndsWith("COMPLETED")) return "<color=green>COMPLETED</color>";
            else
            if(key.EndsWith("STARTED")) return "<color=#436BE8>STARTED</color>";
            else
            if(key.EndsWith("UNDO")) return "<color=orange>UNDO</color>";

            return key;
        }
    }
}