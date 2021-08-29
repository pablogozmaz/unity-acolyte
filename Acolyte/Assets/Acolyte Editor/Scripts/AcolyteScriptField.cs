using System.Text;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.Editor
{
    /// <summary>
    /// Editing field for script source code.
    /// </summary>
    public class AcolyteScriptField : MonoBehaviour
    {
        private const string newLine = "\n";

        private ScriptAsset scriptAsset;

        [SerializeField]
        private float lineMargin = 2;

        [SerializeField]
        private AcolyteLineField lineFieldPrefab;

        [SerializeField]
        private AcolyteContextualizer contextualizer;

        private readonly List<AcolyteLineField> lines = new List<AcolyteLineField>();

        private int poolIndex;

        private AcolyteLineField selectedLine;
        private AcolyteWordField selectedWord;


        public void SetScript(ScriptAsset scriptAsset)
        {
            this.scriptAsset = scriptAsset;
            RenderScript(scriptAsset.Script.Source);
        }

        public void Save()
        {
            scriptAsset.Modify(GetText());
        }

        public string GetText()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < poolIndex; i++)
            {
                Debug.Assert(lines[i].gameObject.activeSelf);

                builder.Append(lines[i].GetText());
                builder.Append(newLine);
            }
            return builder.ToString();
        }

        public void ClearSelection()
        {
            selectedLine = null;

            if(selectedWord != null)
            {
                contextualizer.gameObject.SetActive(false);
                selectedWord.SetDeselected();
                selectedWord = null;
            }
        }

        private void Awake()
        {
            lineFieldPrefab.gameObject.SetActive(false);
        }

        private void RenderScript(string sourceCode)
        {
            var renderUnits = RenderUnitGenerator.GenerateRender(sourceCode, scriptAsset.Script.language);

            Vector2 position = Vector2.zero;

            int i = 0;
            for(; i < renderUnits.Length; i++)
            {
                if(i >= lines.Count)
                    CreateLineField();

                var line = lines[i];

                line.Set(i, renderUnits[i], position, out float height);

                position.y -= height + lineMargin;
            }

            // Pool unused
            poolIndex = i;
            for(; i < lines.Count; i++)
            {
                lines[i].gameObject.SetActive(false);
            }
        }

        private void HandleWordSelection(AcolyteLineField line, AcolyteWordField word)
        {
            if(selectedWord != null)
                selectedWord.SetDeselected();

            selectedLine = line;
            selectedWord = word;
            word.SetSelected();

            contextualizer.Set(GetContextForSelection(), HandleContextSubmit);
            SetContextualizerPosition();
        }

        private WordEditContext GetContextForSelection()
        {
            WordEditContext.IEntry[] entries = new WordEditContext.IEntry[]
            {
                new WordEditContext.Header("Declarations"),
                new WordEditContext.Selectable("Test 1"),
                new WordEditContext.Selectable("Test 2"),

            };

            return new WordEditContext(entries);
        }

        private void HandleContextSubmit(string value)
        {
            for(int i = 0; i < selectedWord.RightSpaces; i++)
                value += " ";
            selectedWord.Text = value;
            RenderScript(GetText());
        }

        private void SetContextualizerPosition()
        {
            Vector2 position = selectedWord.transform.position;
            contextualizer.transform.position = position;
            position = contextualizer.transform.localPosition;
            position.y -= selectedWord.RectTransform.sizeDelta.y + lineMargin;
            contextualizer.transform.localPosition = position;
        }

        private AcolyteLineField CreateLineField()
        {
            var line = Instantiate(lineFieldPrefab, lineFieldPrefab.transform.parent);
            line.OnWordSelection += HandleWordSelection;
            lines.Add(line);
            return line;
        }
    }
}