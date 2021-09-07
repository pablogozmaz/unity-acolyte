using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;


namespace Acolyte.Editor
{
    public class AcolyteWordSelector
    {
        public event Action<AcolyteWordRenderer> OnSelection;

        public AcolyteWordRenderer Selection { get; private set; }

        public string SelectedLine { get; private set; }

        private int lastCaretPosition;


        public void Update(TMP_InputField inputField, AcolyteWordRenderer[] wordRenderers)
        {
            if(lastCaretPosition != inputField.caretPosition)
            {
                FindSelection(inputField, wordRenderers);
                lastCaretPosition = inputField.caretPosition;
            }
        }

        public void ClearSelection()
        {
            if(Selection != null)
            {
                Selection.SetDeselected();
                Selection = null;
                SelectedLine = null;
                OnSelection?.Invoke(null);
            }
        }

        private void FindSelection(TMP_InputField inputField, AcolyteWordRenderer[] wordRenderers)
        {
            if(inputField.caretPosition < inputField.textComponent.textInfo.characterInfo.Length)
            {
                int lineIndex = inputField.textComponent.textInfo.characterInfo[inputField.caretPosition].lineNumber;

                StringBuilder lineBuilder = new StringBuilder();

                for(int i = 0; i < wordRenderers.Length; i++)
                {
                    var wordRenderer = wordRenderers[i];

                    if(wordRenderer.LineIndex < lineIndex) continue;

                    lineBuilder.Append(wordRenderer.Text);

                    bool IsLastWordInTargetLine() => (i + 1) == wordRenderers.Length || wordRenderers[i + 1].LineIndex > lineIndex;

                    if(wordRenderer.IsCharIndexInRange(inputField.caretPosition) || IsLastWordInTargetLine())
                    {
                        Select(wordRenderer, lineBuilder.ToString());
                        return;
                    }
                }
            }

            ClearSelection();
        }

        private void Select(AcolyteWordRenderer wordRenderer, string line)
        {
            if(wordRenderer == Selection)
                return;

            if(Selection != null)
            {
                Selection.SetDeselected();
            }

            SelectedLine = line;

            Selection = wordRenderer;
            Selection.SetSelected();
            OnSelection?.Invoke(Selection);
        }
    }
}