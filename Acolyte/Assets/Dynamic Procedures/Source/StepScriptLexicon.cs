using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures
{
    public sealed partial class StepScriptLexicon : Lexicon
    {
        private AcolyteGameObjectsContainer objectsContainer;

        /// <summary>
        /// The list of actions resulting from processing a script.
        /// </summary>
        private readonly List<StepAction> stepActions = new List<StepAction>();


        public override void EndOfLine() {} // Unused

        protected override void HandleExecutionStart(object context)
        {
            if(context is AcolyteGameObjectsContainer objectsContainer)
                this.objectsContainer = objectsContainer;
            else
                throw new Exception("Invalid type for provided context: "+context.GetType());
        }

        protected override void HandleExecutionCompletion<T>(Action<T> callback)
        {
            callback?.Invoke(stepActions.ToArray() as T);
            objectsContainer = null;
            stepActions.Clear();
        }

        // Called automatically before scene loads
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            OnRequestWords += GenerateCoreModule;
        }

        private static Word[] GenerateCoreModule(Lexicon scope) 
        {
            return scope is StepScriptLexicon stepScriptScope ? stepScriptScope.GenerateCoreModule() : null;
        }

        private Word[] GenerateCoreModule()
        {
            Keyword place = new Keyword("place", SetActionTypePlace);
            Keyword interact = new Keyword("interact", SetActionTypeInteract);

            var selectSingle = new UnityIdentifier<GameObject[]>(SetActionTarget, ()=> { return objectsContainer; });
            var multiPrefix = new Keyword("all");
            var selectMulti = new Literal(SetTargetMultiple);

            var toolPrefix = new Keyword("using");
            var selectTool = new Literal(SetTool);

            place.Then(selectSingle);
            place.Then(multiPrefix);
            interact.Then(selectSingle);
            interact.Then(multiPrefix);

            selectSingle.Then(toolPrefix);
            multiPrefix.Then(selectMulti);
            selectMulti.Then(toolPrefix);

            toolPrefix.Then(selectTool);

            return new Word[] { place, interact };
        }

        private T GetCurrentAction<T>() where T : StepAction
        {
            if(stepActions[stepActions.Count - 1] is T action)
                return action;
            else
                throw new Exception("Invalid type for action retrieval.");
        }

        private void SetActionTypePlace()
        {
            // Add new action
            var action = ScriptableObject.CreateInstance<Examples.MachineOperationAction>();
            stepActions.Add(action);
        }

        private void SetActionTypeInteract()
        {
            // Add new action
            var action = ScriptableObject.CreateInstance<Examples.MachineOperationAction>();
            stepActions.Add(action);
        }

        private void SetActionTarget(GameObject[] gameObjects)
        {
            List<string> parts = new List<string>();
            foreach(var gameObject in gameObjects)
            {
                var interactor = gameObject.GetComponent<Examples.MachinePartInteractor>();
                if(interactor != null)
                {
                    parts.Add(interactor.PropertyName);
                }
            }

            GetCurrentAction<Examples.MachineOperationAction>().partsProperties = parts.ToArray();
        }

        private void SetTargetMultiple(object obj)
        {
        }

        private void SetTool(string obj)
        {
            Debug.Assert(stepActions.Count > 0);

            var action = GetCurrentAction<Examples.MachineOperationAction>();
            action.machineId = "Machine";
            action.interactionControlProperty = "AllowInteraction 1";
        }
    }
}