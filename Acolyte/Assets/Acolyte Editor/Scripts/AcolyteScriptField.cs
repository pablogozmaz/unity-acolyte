using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    /// <summary>
    /// Editing field for script source code.
    /// </summary>
    public class AcolyteScriptField : MonoBehaviour
    {
        public event Action<ScriptAsset> OnScriptSet;

        public const string newLine = "\n";

        public ScriptAsset ScriptAsset { get; private set; }

        public TMP_InputField InputField => inputField;

        private static readonly WaitForEndOfFrame wait = new WaitForEndOfFrame();

        private Script Script => ScriptAsset?.Script;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TMP_Text countField;

        [SerializeField]
        private AcolyteWordRenderer wordRendererPrefab;

        [SerializeField]
        private AcolyteContextualizer contextualizer;

        private AcolyteWordSelector wordSelector = new AcolyteWordSelector();

        private readonly List<AcolyteWordRenderer> wordRenderers = new List<AcolyteWordRenderer>();
        private int renderersPoolIndex;


        public void SetScript(ScriptAsset scriptAsset)
        {
            if(ScriptAsset == scriptAsset)
                return;

            ScriptAsset = scriptAsset;
            inputField.text = scriptAsset.Script.Source;

            OnScriptSet?.Invoke(scriptAsset);
        }

        public void Save()
        {
            ScriptAsset.Modify(inputField.text);
        }
        
        public void ClearSelection() => wordSelector.ClearSelection();
        

        private void Awake()
        {
            wordRendererPrefab.gameObject.SetActive(false);

            countField.text = "";
            inputField.text = "";

            inputField.onValueChanged.AddListener((string value) => 
            {
                RenderScript(value);
            });

            wordSelector.OnSelection += HandleWordSelection;
        }

        private void OnDestroy()
        {
            OnScriptSet = null;

            wordSelector.OnSelection -= HandleWordSelection;
        }

        private void Update()
        {
            AcolyteWordRenderer[] array = new AcolyteWordRenderer[renderersPoolIndex];
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = wordRenderers[i];
            }
            wordSelector.Update(inputField, array);
        }

        private void RenderScript(string sourceCode)
        {
            var renderUnits = RenderUnitGenerator.GenerateRender(sourceCode, Script.language, Script.declexicon);
            SetCountField(renderUnits.Length);
            StartCoroutine(RenderCoroutine(renderUnits));
            inputField.text = sourceCode;
        }

        private void SetCountField(int lineCount)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 1; i <= lineCount; i++)
            {
                sb.Append(i.ToString()).Append("\n");
            }
            countField.text = sb.ToString();
        }

        private IEnumerator RenderCoroutine(RenderUnit[][] renderUnits)
        {
            yield return wait;

            var textInfo = inputField.textComponent.textInfo;

            float yPosition = 0;
            int totalWordCount = 0;
            for(int lineIndex = 0; lineIndex < textInfo.lineCount; lineIndex++)
            {
                var lineInfo = textInfo.lineInfo[lineIndex];
                float lineHeight = lineInfo.lineHeight;

                Vector2 wordPosition = new Vector2(0, yPosition);
                Vector2 wordRectSize = new Vector2(0, lineHeight);

                int wordIndexInLine = 0;

                //Ignore out of bounds
                if(lineIndex >= renderUnits.Length || wordIndexInLine >= renderUnits[lineIndex].Length )
                    continue;

                var renderUnit = renderUnits[lineIndex][wordIndexInLine];

                int charIndexMin = lineInfo.firstCharacterIndex;
                int charIndexMax;
                int wordProgress = 0;
                for(int i = lineInfo.firstCharacterIndex; i < lineInfo.lastCharacterIndex; i++)
                {
                    var charInfo = textInfo.characterInfo[i];

                    if(charInfo.character == renderUnit.content[wordProgress])
                    {
                        wordProgress++;
                        wordRectSize.x += charInfo.xAdvance - charInfo.origin;

                        if(wordProgress == renderUnit.content.Length)
                        {
                            wordProgress = 0;
                            charIndexMax = i;

                            if(totalWordCount >= wordRenderers.Count)
                                CreateWordRenderer();

                            WordRenderParameters parameters = new WordRenderParameters() 
                            {
                                renderUnit = renderUnit,
                                size = wordRectSize,
                                localPosition = wordPosition,
                                lineIndex = lineIndex,
                                wordIndexInLine = wordIndexInLine,
                                charIndexMin = charIndexMin,
                                charIndexMax = charIndexMax
                            };

                            wordRenderers[totalWordCount].Set(parameters);
                            totalWordCount++;

                            wordPosition.x += wordRectSize.x;
                            wordRectSize.x = 0;

                            wordIndexInLine++;
                            charIndexMin = i + 1;

                            if(wordIndexInLine == renderUnits[lineIndex].Length)
                                break;

                            renderUnit = renderUnits[lineIndex][wordIndexInLine];
                        }
                    }
                }

                yPosition -= lineHeight + ParagraphSpacingPixels();
            }

            renderersPoolIndex = totalWordCount;

            // Pool unused
            for(; totalWordCount < wordRenderers.Count; totalWordCount++)
            {
                if(wordRenderers[totalWordCount].gameObject.activeSelf)
                    wordRenderers[totalWordCount].gameObject.SetActive(false);
            }
        }

        private float ParagraphSpacingPixels() => inputField.textComponent.paragraphSpacing * inputField.pointSize * 0.01f;

        private void HandleWordSelection(AcolyteWordRenderer word)
        {
            SetContextualizer();
        }

        private void SetContextualizer()
        {
            if(wordSelector.Selection == null)
            {
                contextualizer.gameObject.SetActive(false);
            }
            else
            {
                var contextParamters = new ContextGenerator.Parameters()
                {
                    line = wordSelector.SelectedLine.Split(Script.language.Separator),
                    word = wordSelector.Selection.Text,
                    wordIndex = wordSelector.Selection.WordIndexInLine,
                    language = Script.language,
                    declexicon = Script.declexicon
                };

                contextualizer.Set(ContextGenerator.GetContext(contextParamters), HandleContextSubmit);
                SetContextualizerPosition();
            }
        }

        private void HandleContextSubmit(string value)
        {
            StringBuilder sb = new StringBuilder();
            string str = inputField.text;
            sb.Append(str.Substring(0, wordSelector.Selection.CharIndexMin));
            sb.Append(ValidateStringSubmit(value));
            sb.Append(str.Substring(wordSelector.Selection.CharIndexMax + 1));

            inputField.text = sb.ToString();
        }

        private string ValidateStringSubmit(string value)
        {
            StringBuilder sb = new StringBuilder();

            string text = wordSelector.Selection.Text;

            for(int i = 0; i < text.Length; i++)
            {
                if(text[i] != ' ') break;
                sb.Append(" ");
            }

            sb.Append(value);

            for(int i = text.Length - 1; i >= 0; i--)
            {
                if(text[i] != ' ') break;
                sb.Append(" ");
            }

            return sb.ToString();
        }

        private void SetContextualizerPosition()
        {
            Vector2 position = wordSelector.Selection.transform.position;
            contextualizer.transform.position = position;
            position = contextualizer.transform.localPosition;
            position.y -= wordSelector.Selection.RectTransform.sizeDelta.y + ParagraphSpacingPixels();
            contextualizer.transform.localPosition = position;
        }

        private AcolyteWordRenderer CreateWordRenderer()
        {
            var wordRenderer = Instantiate(wordRendererPrefab, wordRendererPrefab.transform.parent);
            wordRenderers.Add(wordRenderer);

            return wordRenderer;
        }
    }
}