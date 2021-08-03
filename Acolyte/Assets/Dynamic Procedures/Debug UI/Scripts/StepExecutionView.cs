using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace TFM.DynamicProcedures.DebugUI
{
    public class StepExecutionView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI stepNameField;

        [SerializeField]
        private TextMeshProUGUI stepCompletionStateField;

        [SerializeField]
        private Button autoCompleteButton;

        private StepExecution execution;


        public void SetStepExecution(StepExecution execution) 
        {
            if(this.execution == execution) return;

            if(this.execution != null)
            {
                this.execution.OnObjectStateChange -= UpdateDynamicFields;
            }

            this.execution = execution;

            if(execution != null)
            {
                stepNameField.text = execution.step.name;

                UpdateDynamicFields();
                execution.OnObjectStateChange += UpdateDynamicFields;
            }
            else
            {
                NullifyFields();
            }
        }

        private void Start()
        {
            if(execution == null) NullifyFields();

            autoCompleteButton.onClick.AddListener(TryAutoComplete);
        }

        private void TryAutoComplete() 
        {
            if(execution == null) return;

            execution.AutoComplete();
        }

        private void UpdateDynamicFields() 
        {
            Debug.Assert(execution != null);

            switch(execution.RunningState)
            {
                case ExecutionRunningState.AwaitingStart:
                    stepCompletionStateField.text = "Awainting start";
                    autoCompleteButton.interactable = false;
                    break;

                case ExecutionRunningState.InProcess:
                    stepCompletionStateField.text = "In process";
                    autoCompleteButton.interactable = true;
                    break;

                case ExecutionRunningState.Completed:
                    stepCompletionStateField.text = "<color=#9AE86A>Completed</color>";
                    autoCompleteButton.interactable = false;
                    break;
            }
        }

        private void NullifyFields()
        {
            stepNameField.text = "-";
            stepCompletionStateField.text = "";
            autoCompleteButton.interactable = false;
        }
    }
}