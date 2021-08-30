using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    /// <summary>
    /// Editing field for script source code.
    /// </summary>
    public class AcolyteScriptField_DEP : MonoBehaviour
    {
        public const string newLine = "\n";

        private ScriptAsset scriptAsset;

        private Script Script => scriptAsset?.Script;

        private AcolyteLineField selectedLine;
        private AcolyteWordField selectedWord;

        [SerializeField]
        private AcolyteContextualizer contextualizer;

        [SerializeField]
        private AcolyteLineField lineFieldPrefab;

        [SerializeField]
        private TMP_InputField inputField;

        [Space(8)]
        [SerializeField]
        private float lineMargin = 2;

        private readonly List<AcolyteLineField> lines = new List<AcolyteLineField>();

        private int pooledLinesIndex;



        public void SetScript(ScriptAsset scriptAsset)
        {
            this.scriptAsset = scriptAsset;
            inputField.text = scriptAsset.Script.Source;
        }

        public void Save()
        {
            scriptAsset.Modify(GetText());
        }

        public string GetText()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < pooledLinesIndex; i++)
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

            inputField.onValueChanged.AddListener((string value) => 
            {
                RenderScript(value);
            });
        }

        private void RenderScript(string sourceCode)
        {
            // var renderUnits = RenderUnitGenerator.GenerateRender(sourceCode, Script.language, Script.declexicon);

            var renderUnits = RenderUnitGenerator.GenerateRender(sourceCode, Script.language, Script.declexicon);
            inputField.text = sourceCode;
            /*
            var textInfo = inputField.textComponent.textInfo;
            if(textInfo.characterInfo != null && textInfo.characterInfo.Length > 0)
            {
                Debug.Log(textInfo.characterInfo[0].descender);
            }

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
            pooledLinesIndex = i;
            for(; i < lines.Count; i++)
            {
                lines[i].gameObject.SetActive(false);
            }
            */
        }

        private void HandleWordSelection(AcolyteLineField line, AcolyteWordField word)
        {
            if(selectedWord != null)
                selectedWord.SetDeselected();

            selectedLine = line;
            selectedWord = word;
            selectedWord.SetSelected();

            SetContextualizer(selectedLine, selectedWord);
        }

        private void Deselect()
        {
            contextualizer.gameObject.SetActive(false);

            selectedLine = null;
            if(selectedWord != null)
            {
                selectedWord.SetDeselected();
                selectedWord = null;
            }
        }

        private void SetContextualizer(AcolyteLineField line, AcolyteWordField word)
        {
            var contextParamters = new ContextGenerator.Parameters()
            {
                line = line.GetSplitText(),
                word = word.GetText(),
                wordIndex = word.WordIndex,
                language = Script.language,
                declexicon = Script.declexicon
            };

            contextualizer.Set(ContextGenerator.GetContext(contextParamters), HandleContextSubmit);
            SetContextualizerPosition();
        }

        private void HandleContextSubmit(string value)
        {
            selectedWord.Text = ValidateStringSubmit(value);
            RenderScript(GetText());
            Deselect();
        }

        private string ValidateStringSubmit(string value)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < selectedWord.LeftSpaces; i++)
                sb.Append(" ");
            sb.Append(value);
            for(int i = 0; i < selectedWord.RightSpaces; i++)
                sb.Append(" ");

            return sb.ToString();
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