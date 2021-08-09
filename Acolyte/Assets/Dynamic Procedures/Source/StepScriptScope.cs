using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;
using Acolyte.UnityEngine;


namespace TFM.DynamicProcedures
{
    public sealed partial class StepScriptScope : Scope
    {
        public AcolyteObjectsContext ObjectsContext { get; private set; }


        protected override void HandleExecutionStart(ExecutionContext context)
        {
            ObjectsContext = context.GetSubcontext<AcolyteObjectsContext>();
        }

        protected override void HandleExecutionCompletion()
        {
            ObjectsContext = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() 
        {
            BuildWordTree += (Scope scope) =>
            {
                return scope is StepScriptScope cast ? cast.AddCoreModule() : null;
            };
        }

        private Word[] AddCoreModule()
        {
            Keyword place = new Keyword("place", SetActionTypePlace);
            Keyword interact = new Keyword("interact with", SetActionTypeInteract);

            var selectSingle = new Literal(SetTargetSingle);
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

        public void SetActionTypePlace()
        {
            Debug.Log("Set action type interact");
        }

        public void SetActionTypeInteract()
        {
            Debug.Log("Set action type place");
        }

        public void SetTargetSingle(object obj)
        {
            GameObject foundObject = ObjectsContext.TryGetObject(obj.ToString());

            Debug.Log("Set target single! " + foundObject);
        }

        public void SetTargetMultiple(object obj)
        {
            Debug.Log("Set target multiple! ");
        }

        public void SetTool(object obj)
        {
            Debug.Log("Set tool! " + obj);
        }
    }
}