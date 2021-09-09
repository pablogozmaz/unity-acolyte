using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures
{
    public sealed partial class StepScriptDeclexicon : Declexicon
    {
        /// <summary>
        /// The environment the step is executed upon.
        /// </summary>
        private Environment environment;

        /// <summary>
        /// The list of actions resulting from processing a script.
        /// </summary>
        private readonly List<StepAction> stepActions = new List<StepAction>();


        public override void EndOfLine() {} // Unused

        protected override void HandleExecutionStart(object context)
        {
            if(context is Environment environment)
            {
                this.environment = environment;
            }
            else
                throw new Exception("Invalid type for provided context: " + context.GetType());
        }

        protected override void HandleExecutionCompletion<T>(Action<T> callback)
        {
            callback?.Invoke(stepActions.ToArray() as T);
            stepActions.Clear();
        }

        // Called automatically before scene loads
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            AddDeclexemeFactory((StepScriptDeclexicon declexicon) => 
            {
                return declexicon.GenerateMainWordTree();
            });
        }

        private AcolyteGameObjectsContainer GetObjectsContainer() => environment.ObjectsContainer;
        private AcolyteEntityContainer GetEntityContainer() => environment.EntityContainer;
        private UnityContainerBehaviour<Transform> GetCameraTargetContainer() => environment.CameraTargetContainer;


        private interface ISetter
        {
            public void Set<T>(T value);
        }

        private class PropertySetter : ISetter
        {
            private string name;
            private Entity entity;

            public void Set<T>(T value) 
            {
                if(value is string str)
                {
                    if(name == null)
                        name = str;
                    else
                    {
                        if(entity.Properties.TryGet(name, out var property))
                        {
                            property.SetStringValue(str);
                        }
                    }
                }
                else if(value is Entity entity)
                {
                    this.entity = entity;
                }
                else
                    throw new Exception();
            }
        }

        private class CameraTargetSetter : ISetter
        {
            public void Set<T>(T value)
            {
                if(value is Transform target)
                {
                    var cam = Examples.CameraOrbitBehaviour.Active;
                    if(cam != null)
                        cam.SetTarget(target);
                }
                else
                    throw new Exception();
            }
        }

        private Declexeme[] GenerateMainWordTree()
        {
            ISetter setter = null;

            var set = new Keyword("set");
            var setAs = new Keyword("as");
            var setNewValue = new Literal((string value) => { setter.Set(value); });
            setAs.Then(setNewValue);

            // Property setting
            var setProperty = new Keyword("property", ()=> { setter = new PropertySetter(); });
            var propertyName = new Literal((string value) => { setter.Set(value); });
            var setIn = new Keyword("in");
            var propertyOwnerIdentifier = new UnityIdentifier<Entity>((Entity value) => { setter.Set(value); }, GetEntityContainer);
            set.Then(setProperty);
            setProperty.Then(propertyName);
            propertyName.Then(setIn);
            setIn.Then(propertyOwnerIdentifier);
            propertyOwnerIdentifier.Then(setAs);

            // Camera setting
            var setCamera = new Keyword("camera");
            var cameraTarget = new Keyword("target", () => { setter = new CameraTargetSetter(); });
            var cameraTargetSelect = new UnityIdentifier<Transform>((Transform value) => { setter.Set(value); }, GetCameraTargetContainer);
            var cameraFov = new Keyword("fov");
            // var cameraFovSet = new Literal(setter);
            set.Then(setCamera);
            setCamera.Then(cameraTarget);
            cameraTarget.Then(cameraTargetSelect);
            setCamera.Then(cameraFov);


            // Interaction
            Keyword place = new Keyword("place", CreateActionTypePlace);
            Keyword interact = new Keyword("interact", CreateActionTypeInteract);
            var selectObject = new UnityIdentifier<GameObject[]>(SetActionTarget, GetObjectsContainer);
            Keyword toolPrefix = new Keyword("using");
            var selectTool = new Literal(SetTool);

            place.Then(selectObject);
            interact.Then(selectObject);
            interact.Tolerate("with");

            selectObject.Then(toolPrefix);

            toolPrefix.Then(selectTool);

            // Debug
            Keyword debug = new Keyword("debug");
            Literal debugMessage = new Literal((string message) => { Debug.Log(message); });
            debug.Then(debugMessage);

            return new Declexeme[] { set, place, interact, debug };
        }

        private T GetCurrentAction<T>() where T : StepAction
        {
            if(stepActions[stepActions.Count - 1] is T action)
                return action;
            else
                throw new Exception("Invalid type for action retrieval.");
        }

        private void CreateActionTypePlace()
        {
            // Add new action
            var action = ScriptableObject.CreateInstance<Examples.MachineOperationAction>();
            stepActions.Add(action);
        }

        private void CreateActionTypeInteract()
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

        private void SetTool(string obj)
        {
            Debug.Assert(stepActions.Count > 0);

            var action = GetCurrentAction<Examples.MachineOperationAction>();
            action.machineId = "Machine";
            action.interactionControlProperty = "AllowInteraction1";
        }
    }
}