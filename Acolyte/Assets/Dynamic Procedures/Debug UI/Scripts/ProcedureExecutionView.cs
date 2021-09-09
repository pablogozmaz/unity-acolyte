using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace TFM.DynamicProcedures.DebugUI
{
    public class ProcedureExecutionView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI nameField;

        [SerializeField]
        private TextMeshProUGUI runningStateField;

        [SerializeField]
        private TextMeshProUGUI stepNumberField;

        [SerializeField]
        private TextMeshProUGUI stepAmountField;

        [Space(8)]
        [SerializeField]
        private Button stepBackButton;

        [SerializeField]
        private Button stepForwardButton;

        [Space(16)]
        [SerializeField]
        private StepExecutionView stepExecutionView;

        private ProcedureExecution execution;


        public void SetProcedureExecution(ProcedureExecution execution)
        {
            this.execution = execution;

            nameField.text = execution.procedure.name;

            UpdateDynamicFields();

            execution.OnObjectStateChange.AddListener(UpdateDynamicFields);
        }

        private void Start()
        {
            stepBackButton.onClick.AddListener(TryStepBack);
            stepForwardButton.onClick.AddListener(TryStepForward);
        }

        private void TryStepForward() 
        {
            if(execution == null) return;

            execution.TryStepForward();
        }

        private void TryStepBack()
        {
            if(execution == null) return;

            execution.TryStepBack();
        }

        private void UpdateDynamicFields()
        {
            Debug.Assert(execution != null);

            runningStateField.text = execution.RunningState.ToString();
            stepNumberField.text = (execution.CurrentStepIndex + 1).ToString();
            stepAmountField.text = execution.StepAmount.ToString();

            switch(execution.RunningState)
            {
                case ExecutionRunningState.AwaitingStart:
                    runningStateField.text = "Awaiting start";
                    stepBackButton.interactable = false;
                    stepForwardButton.interactable = true;
                    stepExecutionView.SetStepExecution(null);
                    break;

                case ExecutionRunningState.InProcess:
                    runningStateField.text = "In process";
                    stepBackButton.interactable = true && execution.settings.canStepBackwards;
                    stepForwardButton.interactable = execution.CurrentStepExecution.IsCompleted;
                    stepExecutionView.SetStepExecution(execution.CurrentStepExecution);
                    break;

                case ExecutionRunningState.Completed:
                    runningStateField.text = "<color=#9AE86A>Completed</color>";
                    stepBackButton.interactable = false;
                    stepForwardButton.interactable = false;
                    stepExecutionView.SetStepExecution(execution.CurrentStepExecution);
                    break;

                case ExecutionRunningState.Cancelled:
                    runningStateField.text = "<color=orange>Cancelled</color>";
                    stepBackButton.interactable = false;
                    stepForwardButton.interactable = false;
                    stepExecutionView.SetStepExecution(null);
                    break;
            }
        }
    }
}