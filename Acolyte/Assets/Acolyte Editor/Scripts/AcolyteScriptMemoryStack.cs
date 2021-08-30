using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.Editor
{
    public class AcolyteScriptMemoryStack : MonoBehaviour
    {
        private readonly Dictionary<ScriptAsset, Stack<string>> stacks = new Dictionary<ScriptAsset, Stack<string>>();

        [SerializeField]
        private AcolyteScriptField scriptField;

        private string lastValue;



        private void Awake()
        {
            scriptField.OnScriptSet += HandleScriptSet;
            HandleScriptSet(scriptField.ScriptAsset);
        }

        private void HandleScriptSet(ScriptAsset script) 
        {
            if(script == null)
                return;

            if(!stacks.TryGetValue(script, out var stack))
            {
                stack = new Stack<string>();
                stacks.Add(script, stack);
            }

            lastValue = scriptField.InputField.text;
            stack.Push(lastValue);
        }

        private void Update()
        {
            if(scriptField.ScriptAsset == null)
                return;

            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            {
                if(stacks.TryGetValue(scriptField.ScriptAsset, out var stack))
                {
                    if(stack.Count > 0)
                    {
                        lastValue = stack.Pop();
                        scriptField.InputField.text = lastValue;
                    }
                }
            }

            else if(lastValue != scriptField.InputField.text)
            {
                SaveValue(scriptField.ScriptAsset, lastValue);
                lastValue = scriptField.InputField.text;
            }
        }

        private void SaveValue(ScriptAsset sa, string text)
        {
            if(stacks.TryGetValue(sa, out var stack))
            {
                stack.Push(text);
            }
        }
    }
}