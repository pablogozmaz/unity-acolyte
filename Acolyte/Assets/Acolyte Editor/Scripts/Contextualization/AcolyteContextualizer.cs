using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.Editor
{
    [RequireComponent(typeof(RectTransform))]
    public class AcolyteContextualizer : MonoBehaviour
    {
        public RectTransform RectTransform { get { return rectTransform; } }

        [SerializeField, HideInInspector]
        private RectTransform rectTransform;

        [SerializeField]
        private AcolyteContextSelectable selectablePrefab;

        private List<AcolyteContextSelectable> selectables = new List<AcolyteContextSelectable>();

        private Action<string> submitCallback;


        public void Set(WordEditContext editContext, Action<string> submitCallback)
        {
            if(!gameObject.activeSelf)
                gameObject.SetActive(true);

            this.submitCallback = submitCallback;

            int selectableIndex = 0;
            foreach(var entry in editContext.Entries)
            {
                if(entry is WordEditContext.Selectable selectable)
                {
                    if(selectableIndex >= selectables.Count)
                        CreateSelectable();

                    var selectableObj = selectables[selectableIndex];

                    if(!selectableObj.gameObject.activeSelf)
                        selectableObj.gameObject.SetActive(true);

                    selectableObj.SetOption(selectable.Text);

                    selectableIndex++;
                }
            }

            // Pool
            for(; selectableIndex < selectables.Count; selectableIndex++)
            {
                if(selectables[selectableIndex].gameObject.activeSelf)
                    selectables[selectableIndex].gameObject.SetActive(false);
            }
        }

        private void Awake() 
        {
            selectablePrefab.gameObject.SetActive(false);
        }

        private void Update()
        {
            ProcessDefaultInput();
        }

        private AcolyteContextSelectable CreateSelectable() 
        {
            var option = Instantiate(selectablePrefab, selectablePrefab.transform.parent);
            option.OnSubmit += HandleOptionSubmit;
            selectables.Add(option);
            return option;
        }

        private void HandleOptionSubmit(string value)
        {
            submitCallback.Invoke(value);
            gameObject.SetActive(false);
        }

        private void ProcessDefaultInput() 
        {
            // Cancel selection
            if(Input.GetKeyDown(KeyCode.Escape))
            {

            }
            // Try delete word
            else if(Input.GetKeyDown(KeyCode.Delete))
            {

            }
            // Next?
            else if(Input.GetKeyDown(KeyCode.Space))
            {

            }
        }

        private void OnValidate()
        {
            rectTransform = transform as RectTransform;
        }
    }
}