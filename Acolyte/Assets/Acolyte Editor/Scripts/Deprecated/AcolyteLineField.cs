using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    [RequireComponent(typeof(RectTransform))]
    public class AcolyteLineField : MonoBehaviour
    {
        public event Action<AcolyteLineField, AcolyteWordField> OnWordSelection;

        public int LineIndex { get; private set; }

        [SerializeField, HideInInspector]
        private RectTransform rectTransform;

        [SerializeField]
        private AcolyteWordField wordFieldPrefab;

        [SerializeField]
        private TMP_Text countText;

        private readonly List<AcolyteWordField> wordFields = new List<AcolyteWordField>();

        private int poolIndex;


        public string GetText()
        {
            StringBuilder builder = new StringBuilder();

            for(int i = 0; i < poolIndex; i++)
            {
                Debug.Assert(wordFields[i].gameObject.activeSelf);

                builder.Append(wordFields[i].GetText());
            }

            return builder.ToString();
        }

        public string[] GetSplitText() 
        {
            string[] array = new string[poolIndex];

            for(int i = 0; i < array.Length; i++)
            {
                array[i] = wordFields[i].GetText();
            }

            return array;
        }

        public void Set(int lineIndex, RenderUnit[] renderUnits, Vector2 position, out float height)
        {
            LineIndex = lineIndex;
            countText.text = (++lineIndex).ToString();
            rectTransform.localPosition = position;

            SetWords(renderUnits, out height);

            if(!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        private void SetWords(RenderUnit[] renderUnits, out float height)
        {
            Vector2 wordPosition = Vector2.zero;
            float totalWidth = 0;
            height = 0;

            int i = 0;
            for(; i < renderUnits.Length; i++)
            {
                if(i >= wordFields.Count)
                    CreateField();

                WordFieldParameters parameters = new WordFieldParameters()
                {
                    renderUnit = renderUnits[i],
                    localPosition = wordPosition,
                    wordIndex = i
                };

                wordFields[i].Set(parameters, out Vector2 size);

                wordPosition.x += size.x;
                totalWidth += size.x;

                if(size.y > height)
                    height = size.y;
            }

            // Pool unused
            poolIndex = i;
            for(; i < wordFields.Count; i++)
            {
                wordFields[i].gameObject.SetActive(false);
            }
        }

        private void HandleWordFieldSelection(AcolyteWordField wordField)
        {
            OnWordSelection?.Invoke(this, wordField);
        }

        private AcolyteWordField CreateField()
        {
            var field = Instantiate(wordFieldPrefab, transform);
            field.OnWordSelection += HandleWordFieldSelection;
            wordFields.Add(field);
            return field;
        }

        private void Awake()
        {
            wordFieldPrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            OnWordSelection = null;
        }

        private void OnValidate()
        {
            rectTransform = transform as RectTransform;
        }
    }
}