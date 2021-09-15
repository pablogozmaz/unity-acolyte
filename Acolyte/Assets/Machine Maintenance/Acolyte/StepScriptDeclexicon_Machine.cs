using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace ProcedUnity
{
    public partial class StepScriptDeclexicon
    {
        private ToolsContainer GetToolsContainer() => ToolsContainer.Instance;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeMachineModule()
        {
            AddDeclexemeFactory((StepScriptDeclexicon declexicon) =>
            {
                return declexicon.GenerateMachineWordTree();
            });
        }

        private Declexeme[] GenerateMachineWordTree() 
        {
            // Interaction
            Keyword interact = new Keyword("interact", CreateActionTypeInteract);
            var selectObject = new Identifier<GameObject[]>(SetInteractionTarget, GetObjectsContainer);
            Keyword toolPrefix = new Keyword("using");
            var selectTool = new Identifier<Tool>(SetInteractionTool, GetToolsContainer);
            interact.Then(selectObject);
            interact.Tolerate("with");
            selectObject.Then(toolPrefix);
            toolPrefix.Then(selectTool);

            return new Declexeme[] { interact };
        }

        private void CreateActionTypeInteract()
        {
            var action = ScriptableObject.CreateInstance<InteractionAction>();
            stepActions.Add(action);
        }

        private void SetInteractionTarget(GameObject[] gameObjects)
        {
            List<Interactor> interactors = new List<Interactor>();
            foreach(var gameObject in gameObjects)
            {
                var interactorArray = gameObject.GetComponentsInChildren<Interactor>();
                if(interactorArray != null)
                    interactors.AddRange(interactorArray);
            }

            GetCurrentAction<InteractionAction>().interactors = interactors.ToArray();
        }

        private void SetInteractionTool(Tool tool)
        {
            Debug.Assert(stepActions.Count > 0);

            var action = GetCurrentAction<InteractionAction>();
            if(action != null)
            {
                action.tool = tool;
            }
        }
    }
}