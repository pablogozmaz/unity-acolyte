using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ProcedUnity
{
    public class ToolNull : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent OnSelected;

        [SerializeField]
        private UnityEvent OnDeselected;


        public void Select()
        {
            Tool.ClearSelection();
        }

        private void Awake()
        {
            HandleToolSelection();

            Tool.OnSelection += HandleToolSelection;
        }

        private void OnDestroy()
        {
            Tool.OnSelection -= HandleToolSelection;
        }

        private void HandleToolSelection()
        {
            if(Tool.Current == null)
                OnSelected.Invoke();
            else
                OnDeselected.Invoke();
        }
    }
}