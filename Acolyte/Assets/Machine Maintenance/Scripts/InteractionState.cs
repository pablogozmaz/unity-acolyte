using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    public enum InteractionStates
    {
        NonInteractable,
        Interactable,
        Interacted,
        InteractedIncorrect
    }

    public class InteractionState : Property<InteractionStates>
    {
        public event Action<InteractionState> OnValueChanged;

        public override PropertyType Type => PropertyType.CustomStruct;


        public InteractionState(string name, string initialValue) : base(name, initialValue) {}

        public InteractionState(string name, InteractionStates initialValue) : base(name, initialValue) {}


        protected override void InvokeOnValueChangedDerived()
        {
            OnValueChanged?.Invoke(this);
        }

        protected override bool TryParseValueFromText(string valueAsText, out InteractionStates result)
        {
            return Enum.TryParse(valueAsText, out result);
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AddParsing() 
        {
            PropertyParser.AddCustomFactory("istate", (string name, string value) =>
            {
                return new InteractionState(name, value);
            });
        }
    }
}