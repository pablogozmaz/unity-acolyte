using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Binds all existing StepAction types to their implementing StepActionExecution type.
    /// <para>
    /// Binding data is persistently stored in a file, whose content is obtained <b>on the editor</b> via assembly reflection.
    /// </para>
    /// </summary>
    public class StepActionExecutionTypeBinder
    {
        private const string bindingFileName = "ProcedureStepActionExecutionsBinding";

        private static Dictionary<Type, Type> stepActionToExecutionBinding;


        public static bool TryGetExecutionType(Type stepActionType, out Type executionType) 
        {
            if(stepActionToExecutionBinding == null)
                SetStepActionToStepActionExecutionTypeBinding();

            return stepActionToExecutionBinding.TryGetValue(stepActionType, out executionType);
        }

        private static void SetStepActionToStepActionExecutionTypeBinding() 
        {
            stepActionToExecutionBinding = new Dictionary<Type, Type>();

            TryLoadBindingFile();
        }

        private static bool TryLoadBindingFile()
        {
            var jsonAsset = Resources.Load<TextAsset>(bindingFileName);

            if(jsonAsset == null) return false;

            stepActionToExecutionBinding = JsonConvert.DeserializeObject<Dictionary<Type, Type>>(jsonAsset.text);

            return true;
        }

#if UNITY_EDITOR
        // Ensure binding is also performed on build
        private class StepActionexecutionTypeBinderPreprocess : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
                ProduceBindingToFile();
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void ProduceBindingToFile()
        {
            if(stepActionToExecutionBinding == null) stepActionToExecutionBinding = new Dictionary<Type, Type>();

            BindAllExistingexecutionTypes(stepActionToExecutionBinding);
            SaveBindingFile(stepActionToExecutionBinding);
        }

        private static void SaveBindingFile(Dictionary<Type, Type> binding)
        {
            string json = JsonConvert.SerializeObject(binding);

            TextAsset jsonAsset = new TextAsset(json);
            EnsureResourcesDirectoryExists();
            AssetDatabase.CreateAsset(jsonAsset, "Assets/Resources/"+ bindingFileName+".asset");
        }

        private static void EnsureResourcesDirectoryExists()
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
        }

        private static void BindAllExistingexecutionTypes(Dictionary<Type, Type> binding)
        {
            IEnumerable<Type> types = GetAllexecutionTypes();

            if(types != null)
            {
                foreach(var foundexecutionType in types)
                {
                    TryBindStepActionexecution(foundexecutionType, binding);
                }
            }
        }

        private static IEnumerable<Type> GetAllexecutionTypes()
        {
            return typeof(StepActionExecution).Assembly.GetTypes().Where(t => IsTypeValid(t));
        }

        private static bool IsTypeValid(Type t)
        {
            return typeof(StepActionExecution).IsAssignableFrom(t)
                && !t.IsAbstract
                && !t.IsInterface
                && !t.IsGenericType;
        }

        private static void TryBindStepActionexecution(Type executionType, Dictionary<Type, Type> binding)
        {
            var execution = (StepActionExecution)Activator.CreateInstance(executionType);

            Type stepActionType = execution.StepActionType;

            Debug.Assert(!IsStepActionAlreadyBinded(binding, stepActionType),
                "A StepAction is being implemented by more than one StepAction execution: " + stepActionType);

            binding.Add(stepActionType, executionType);
        }

        private static bool IsStepActionAlreadyBinded(Dictionary<Type, Type> binding, Type stepActionType)
        {
            return binding.ContainsKey(stepActionType);
        }
#endif
    }
}