using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity.DebugUI
{
    public class MainView : MonoBehaviour
    {
        [SerializeField]
        private KeyCode hotKey = KeyCode.Backslash;

        [SerializeField]
        private GameObject root;

        [SerializeField]
        private bool startOpen = true;

        [Space(12)]
        [SerializeField]
        private EnvironmentView environmentView;

        [SerializeField]
        private ProcedureExecutionView procedureExecutionView;


        private void Awake()
        {
            SetRootActive(startOpen);

            Environment.OnEnvironmentInitialized += AddEnvironment;
            ProcedureExecution.OnExecutionCreated += SetProcedureexecution;
        }

        private void OnDestroy()
        {
            Environment.OnEnvironmentInitialized -= AddEnvironment;
            ProcedureExecution.OnExecutionCreated -= SetProcedureexecution;
        }

        private void SetProcedureexecution(ProcedureExecution execution) 
        {
            procedureExecutionView.SetProcedureExecution(execution);
        }

        private void AddEnvironment(Environment environment)
        {
            environmentView.SetEnvironment(environment);
        }

        private void SetRootActive(bool doSetActive) 
        {
            if(root.activeSelf != doSetActive) root.SetActive(doSetActive);
        }

        private void Update()
        {
            if(Input.GetKeyDown(hotKey))
            {
                SetRootActive(!root.activeSelf);
            }
        }
    }
}