using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    public class AcolyteWordSelector
    {
        public event Action<AcolyteWordRenderer> OnSelection;

        public AcolyteWordRenderer Selection { get; private set; }

        public string SelectedLine { get; private set; }

        public bool IsSelectionInsideRightSpacing { get; private set; }

        public AcolyteWordRenderer[] wordRenderers;

        private readonly MonoBehaviour owner;
        private readonly TMP_InputField inputField;

        private int lastCaretPosition = -1;



        public AcolyteWordSelector(MonoBehaviour owner, TMP_InputField inputField)
        {
            this.owner = owner;
            this.inputField = inputField;
        }

        public void Update()
        {
            if(lastCaretPosition != inputField.caretPosition)
            {
                owner.StartCoroutine(FindSelection());
                lastCaretPosition = inputField.caretPosition;
            }
        }

        public void ClearSelection()
        {
            if(Selection != null)
            {
                lastCaretPosition = -1;
                Selection.SetDeselected();
                Selection = null;
                SelectedLine = null;
                OnSelection?.Invoke(null);
            }
        }

        private IEnumerator FindSelection()
        {
            yield return null;

            bool success = false;

            if(wordRenderers != null && wordRenderers.Length > 0)
            {
                if(inputField.caretPosition < inputField.textComponent.textInfo.characterInfo.Length)
                {
                    int lineIndex = inputField.textComponent.textInfo.characterInfo[inputField.caretPosition].lineNumber;

                    StringBuilder lineBuilder = new StringBuilder();

                    for(int i = 0; i < wordRenderers.Length; i++)
                    {
                        var wordRenderer = wordRenderers[i];

                        // Ignore all previous lines
                        if(wordRenderer.LineIndex < lineIndex) continue;

                        lineBuilder.Append(wordRenderer.Text);

                        // Ignore all previous words
                        if(inputField.caretPosition < wordRenderer.CharIndexMin) continue;
                        
                        if(inputField.caretPosition <= wordRenderer.CharIndexMax)
                        {
                            Select(wordRenderer, lineBuilder.ToString());
                            success = true;
                        }
                        else
                        {
                            bool isLastWordInTargetLine = (i + 1) == wordRenderers.Length || wordRenderers[i + 1].LineIndex > lineIndex;
                            if(isLastWordInTargetLine)
                            {
                                Select(wordRenderer, lineBuilder.ToString());
                                success = true;
                            }
                        }
                    }
                }
            }

            if(!success)
                ClearSelection();
        }

        private void Select(AcolyteWordRenderer wordRenderer, string line)
        {
            if(wordRenderer == Selection)
            {
                if(wordRenderer != null)
                {
                    // Always update line
                    SelectedLine = line;

                    IsSelectionInsideRightSpacing = false;

                    // Try process when caret is on last position with spaces
                    if(inputField.caretPosition > wordRenderer.CharIndexMax)
                    {
                        int max = wordRenderer.CharIndexMax;
                        int min = wordRenderer.CharIndexMin;
                        int index = (max - min);

                        if(wordRenderer.Text[index] == ' ')
                        {
                            IsSelectionInsideRightSpacing = true;
                        }
                    }

                    OnSelection?.Invoke(Selection);
                }

                return;
            }

            if(Selection != null)
            {
                Selection.SetDeselected();
            }


            SelectedLine = line;
            Selection = wordRenderer;
            Selection.SetSelected();
            IsSelectionInsideRightSpacing = false;
            OnSelection?.Invoke(Selection);
        }
    }
}