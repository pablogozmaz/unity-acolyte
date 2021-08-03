using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte.UnityEngine;


namespace Acolyte.Examples
{
    public class ExampleAcolyteBehaviour : AcolyteBehaviour
    {
        protected override Language CreateLanguage() 
        {
            return new Language("PSDL", new PSDLActionScope())
            {
                isCaseSensitive = false,
                supportsInvalidWords = true,
            };
        }

        private void Start()
        {
            // TEST
            Script script = new Script("place all 'tornillos' using 'herramienta'", Language, true);

            ExecuteScript(script);
        }
    }

    public class PSDLActionScope : InvocationScope
    {
        public void SetActionType(ScriptActionContext actionContext)
        {
            Debug.Log("Set action type!");
        }

        public void SetTargetSingle(ScriptActionContext actionContext, object obj)
        {
            Debug.Log("Set target single! " + obj);
        }

        public void SetTargetMultiple(ScriptActionContext actionContext, object obj)
        {
            Debug.Log("Set target multiple! " + obj);
        }

        public void SetTool(ScriptActionContext actionContext, object obj)
        {
            Debug.Log("Set tool! " + obj);
        }

        protected override Word[] Build()
        {
            Keyword place = new Keyword("place", SetActionType);
            Keyword interact = new Keyword("interact with", SetActionType);

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
    }
}