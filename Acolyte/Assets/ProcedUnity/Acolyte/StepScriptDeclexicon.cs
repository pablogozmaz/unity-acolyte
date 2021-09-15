using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace ProcedUnity
{
    public sealed partial class StepScriptDeclexicon : Declexicon
    {
        public static Environment environment;

        /// <summary>
        /// The list of actions resulting from processing a script.
        /// </summary>
        private readonly List<StepAction> stepActions = new List<StepAction>();

        private string propertyName;
        private Entity propertyOwner;
        private Event selectedEvent;

        private readonly Dictionary<Event, Action> eventRegistry = new Dictionary<Event, Action>();


        public override void EndOfLine() {} // Unused


        public override void HandleRecompilation()
        {
            foreach(var pair in eventRegistry)
                pair.Key.action -= pair.Value;
            
            eventRegistry.Clear();
        }

        protected override void HandleExecutionStart(object context) {} // Unused

        protected override void HandleExecutionCompletion<T>(Action<T> callback)
        {
            callback?.Invoke(stepActions.ToArray() as T);
            stepActions.Clear();
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            AddDeclexemeFactory((StepScriptDeclexicon declexicon) => 
            {
                return declexicon.GenerateMainWordTree();
            });
        }

        private Environment GetEnvironment() => environment;
        private AcolyteGameObjectsContainer GetObjectsContainer() => environment.ObjectsContainer;
        private UnityContainerBehaviour<Transform> GetCameraTargetContainer() => environment.CameraTargetContainer;
        private EventsContainer GetEventsContainer() => EventsContainer.Instance;


        private Declexeme[] GenerateMainWordTree()
        {
            var set = new Keyword("set");

            // Property setting
            var setProperty = new Keyword("property");
            var propertyName = new Literal(SetPropertyName);
            var setIn = new Keyword("in");
            var propertyOwnerIdentifier = new Identifier<Entity>(SetPropertyOwner, GetEnvironment);
            var setPropertyAs = new Keyword("as");
            var setPropertyValue = new Literal(SetPropertyValue);
            set.Then(setProperty);
            setProperty.Then(propertyName);
            propertyName.Then(setIn);
            setIn.Then(propertyOwnerIdentifier);
            propertyOwnerIdentifier.Then(setPropertyAs);
            setPropertyAs.Then(setPropertyValue);

            // Camera setting
            var setCamera = new Keyword("camera");
            var cameraTarget = new Keyword("target");
            var cameraTargetSelect = new Identifier<Transform>(SetCameraTarget, GetCameraTargetContainer);
            set.Then(setCamera);
            setCamera.Then(cameraTarget);
            cameraTarget.Then(cameraTargetSelect);

            // Events
            var when = new Keyword("when");
            var selectEvent = new Identifier<Event>(SelectEvent, GetEventsContainer);
            var eventAlert = new Keyword("alert");
            var eventAlertText = new Literal(EventAlert);
            when.Then(selectEvent);
            selectEvent.Then(eventAlert);
            eventAlert.Then(eventAlertText);

            // Debug
            Keyword debug = new Keyword("debug");
            Literal debugMessage = new Literal(DebugMessage);
            debug.Then(debugMessage);

            return new Declexeme[] { set, debug, when };
        }

        private void DebugMessage(string message)
        {
            Debug.Log(message);
        }

        private void SetPropertyOwner(Entity entity)
        {
            propertyOwner = entity;
        }

        private void SetPropertyName(string value)
        {
            propertyName = value;
        }

        private void SetCameraTarget(Transform transform)
        {
            var cam = Examples.CameraOrbitBehaviour.Active;
            if(cam != null)
                cam.SetTarget(transform);
        }

        private void SetPropertyValue(string value) 
        {
            if(propertyOwner != null && propertyName != null)
                if(propertyOwner.Properties.TryGet(propertyName, out var property))
                    property.SetStringValue(value);
        }

        private T GetCurrentAction<T>() where T : StepAction
        {
            if(stepActions[stepActions.Count - 1] is T action)
                return action;
            else
                throw new Exception("Invalid type for action retrieval.");
        }

        private void SelectEvent(Event ev) => this.selectedEvent = ev;
        

        private void EventAlert(string alert)
        {
            if(selectedEvent == null) return;

            void action() { Alert.ShowAlert(alert); }

            eventRegistry.Add(selectedEvent, action);

            selectedEvent.action += action;
        }
    }
}