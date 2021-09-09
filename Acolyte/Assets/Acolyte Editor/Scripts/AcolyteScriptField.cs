using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


namespace Acolyte.Editor
{
    /// <summary>
    /// Editing field for script source code.
    /// </summary>
    public class AcolyteScriptField : MonoBehaviour
    {
        public event Action<ScriptAsset> OnScriptSet;

        public const char newLine = '\n';

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

        [SerializeField]
        private RectTransform selectionHighlight;

        [Space(8)]
        [SerializeField]
        private UnityEvent OnHasScript;

        [SerializeField]
        private UnityEvent OnHasNoScript;

        private AcolyteWordSelector wordSelector;

        private readonly List<AcolyteWordRenderer> wordRenderers = new List<AcolyteWordRenderer>();
        private int renderersPoolIndex;


        public void SetScript(ScriptAsset scriptAsset)
        {
            if(ScriptAsset == scriptAsset)
                return;

            ScriptAsset = scriptAsset;

            if(ScriptAsset != null)
            {
                inputField.text = scriptAsset.Script.Source;

                OnScriptSet?.Invoke(scriptAsset);
                OnHasScript.Invoke();
            }
            else
                OnHasNoScript.Invoke();
        }

        public void Save()
        {
            if(inputField.text == ScriptAsset.Script.Source)
                return;

            ScriptAsset.Modify(inputField.text);
        }
        
        public void ClearSelection() => wordSelector.ClearSelection();
        

        private void Awake()
        {
            OnHasNoScript.Invoke();

            wordRendererPrefab.gameObject.SetActive(false);
            selectionHighlight.gameObject.SetActive(false);

            countField.text = "";
            inputField.text = "";

            inputField.onValueChanged.AddListener((string value) => 
            {
                if(ScriptAsset == null)
                    return;

                RenderScript(value);
            });

            InputField.onSelect.AddListener((string ignore) => 
            {
                SetContextualizer(true);
            });

            wordSelector = new AcolyteWordSelector(this, inputField);

            wordSelector.OnSelection += HandleWordSelection;
            contextualizer.OnSubmit += HandleContextSubmit;
        }

        private void OnDestroy()
        {
            OnScriptSet = null;

            wordSelector.OnSelection -= HandleWordSelection;
            contextualizer.OnSubmit -= HandleContextSubmit;
        }

        private void Update()
        {
            wordSelector.Update();
        }

        private void RenderScript(string sourceCode)
        {
            var renderUnits = RenderUnitGenerator.GenerateRender(sourceCode, Script.language, Script.declexicon);
            StartCoroutine(RenderCoroutine(renderUnits));
            inputField.text = sourceCode;

            int lineCount = sourceCode.Count(c => c == newLine) + 1;
            SetCountField(lineCount > 0 ? lineCount : 1);
        }

        private void SetCountField(int lineCount)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 1; i <= lineCount; i++)
            {
                sb.Append(i.ToString()).Append(newLine);
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
                if(lineIndex < renderUnits.Length && wordIndexInLine < renderUnits[lineIndex].Length)
                {
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

            // Send array to selector
            AcolyteWordRenderer[] array = new AcolyteWordRenderer[renderersPoolIndex];
            for(int i = 0; i < array.Length; i++)
                array[i] = wordRenderers[i];
            wordSelector.wordRenderers = array;

            // Set highlight after render to ensure correct size
            SetSelectionHighlight();
        }

        private float ParagraphSpacingPixels() => inputField.textComponent.paragraphSpacing * inputField.pointSize * 0.01f;

        private void HandleWordSelection(AcolyteWordRenderer word)
        {
            SetContextualizer();
            SetSelectionHighlight();
        }

        private void SetContextualizer(bool forceContext = false)
        {
            if(!forceContext && wordSelector.Selection == null && !inputField.isFocused)
            {
                contextualizer.Set(null);
            }
            else
            {
                string line;
                string word;
                int wordIndex;

                if(wordSelector.Selection != null)
                {
                    line = wordSelector.SelectedLine;
                    word = wordSelector.Selection.Text;
                    wordIndex = wordSelector.Selection.WordIndexInLine;

                    if(wordSelector.IsSelectionInsideRightSpacing) // Consider spacing as next context
                    {
                        wordIndex++;
                    }
                }
                else // If there is no selection due to empty string, force context
                {
                    line = "";
                    word = "";
                    wordIndex = 0;
                }

                var contextParamters = new ContextGenerator.Parameters()
                {
                    language = Script.language,
                    declexicon = Script.declexicon,
                    line = line,
                    word = word,
                    wordIndex = wordIndex
                };

                var editContext = ContextGenerator.GetContext(contextParamters);

                if(editContext.EntriesCount > 0)
                {
                    contextualizer.Set(editContext);
                }
                else
                {
                    contextualizer.Set(null);
                }
            }
        }

        private void SetSelectionHighlight()
        {
            if(wordSelector.Selection == null)
            {
                if(selectionHighlight.gameObject.activeSelf)
                    selectionHighlight.gameObject.SetActive(false);
            }
            else
            {
                if(!selectionHighlight.gameObject.activeSelf)
                    selectionHighlight.gameObject.SetActive(true);


                Vector2 pos = wordSelector.Selection.RectTransform.anchoredPosition;
                pos.x = Mathf.Round(pos.x); // Round for pixel perfect
                pos.y = Mathf.Round(pos.y);
                selectionHighlight.anchoredPosition = pos;
                float sizeX = wordSelector.Selection.TextField.preferredWidth;
                float sizeY = wordSelector.Selection.RectTransform.sizeDelta.y;
                selectionHighlight.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Round(sizeX));
                selectionHighlight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Round(sizeY));
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

            SetSelectionHighlight();
        }

        private string ValidateStringSubmit(string value)
        {
            StringBuilder sb = new StringBuilder();

            string text = wordSelector.Selection.Text;

            // Add spaces from left to right
            for(int i = 0; i < text.Length; i++)
            {
                if(text[i] != ' ') break;
                sb.Append(" ");
            }

            sb.Append(value);


            // Add spaces from right to left
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