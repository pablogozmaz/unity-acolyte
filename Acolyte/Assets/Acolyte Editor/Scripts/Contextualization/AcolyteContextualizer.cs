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
        private AcolyteContextHeader headerPrefab;

        [SerializeField]
        private AcolyteContextSelectable selectablePrefab;

        private readonly List<AcolyteContextHeader> headers = new List<AcolyteContextHeader>();
        private readonly List<AcolyteContextSelectable> selectables = new List<AcolyteContextSelectable>();

        private Action<string> submitCallback;


        public void Set(WordEditContext editContext, Action<string> submitCallback)
        {
            if(!gameObject.activeSelf)
                gameObject.SetActive(true);

            this.submitCallback = submitCallback;

            int headerIndex = 0;
            int selectableIndex = 0;
            foreach(var entry in editContext.Entries)
            {
                if(entry is WordEditContext.Header header)
                {
                    if(headerIndex >= headers.Count)
                        CreateHeader();

                    headers[headerIndex].SetHeader(header.Text);
                    headerIndex++;
                }
                else if(entry is WordEditContext.Selectable selectable)
                {
                    if(selectableIndex >= selectables.Count)
                        CreateSelectable();

                    selectables[selectableIndex].SetOption(selectable.Text);
                    selectableIndex++;
                }
            }

            // Pool
            for(; headerIndex < headers.Count; headerIndex++)
            {
                if(headers[headerIndex].gameObject.activeSelf)
                    headers[headerIndex].gameObject.SetActive(false);
            }
            for(; selectableIndex < selectables.Count; selectableIndex++)
            {
                if(selectables[selectableIndex].gameObject.activeSelf)
                    selectables[selectableIndex].gameObject.SetActive(false);
            }
        }

        private void Awake() 
        {
            headerPrefab.gameObject.SetActive(false);
            selectablePrefab.gameObject.SetActive(false);
        }

        private void Update()
        {
            ProcessDefaultInput();
        }

        private AcolyteContextSelectable CreateSelectable() 
        {
            var selectable = Instantiate(selectablePrefab, selectablePrefab.transform.parent);
            selectable.OnSubmit += HandleOptionSubmit;
            selectables.Add(selectable);
            return selectable;
        }

        private AcolyteContextHeader CreateHeader()
        {
            var header = Instantiate(headerPrefab, headerPrefab.transform.parent);
            headers.Add(header);
            return header;
        }

        private void HandleOptionSubmit(string value)
        {
            submitCallback.Invoke(value);
        }

        private void ProcessDefaultInput() 
        {
            // Cancel selection
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
            }
        }

        private void OnValidate()
        {
            rectTransform = transform as RectTransform;
        }
    }
}