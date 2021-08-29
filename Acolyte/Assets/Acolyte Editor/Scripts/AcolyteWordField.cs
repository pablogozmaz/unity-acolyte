using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


namespace Acolyte.Editor
{
    public struct WordFieldParameters
    {
        public RenderUnit renderUnit;
        public Vector2 localPosition;
        public int wordIndex;
    }

    [RequireComponent(typeof(RectTransform))]
    public class AcolyteWordField : MonoBehaviour
    {
        public event Action<AcolyteWordField> OnWordSelection;

        public RectTransform RectTransform => rectTransform;

        public int WordIndex { get; private set; }

        public string Text 
        { 
            get { return textField.text; }
            set { textField.text = value; }
        }

        public int RightSpaces { get; private set; }

        private const float fontSizeSpaceMul = 0.32f;

        [SerializeField, HideInInspector]
        private RectTransform rectTransform;

        [SerializeField]
        private TMP_Text textField;

        [SerializeField]
        private Image highlighter;

        [Space(8)]
        [SerializeField]
        private UnityEvent OnSelected;
        [SerializeField]
        private UnityEvent OnDeselected;


        public string GetText() 
        {
            return textField.text;
        }

        public void Set(WordFieldParameters parameters, out Vector2 size)
        {
            textField.text = parameters.renderUnit.content;
            SetRightSpaces();

            SetSize(out size);
            rectTransform.localPosition = parameters.localPosition;

            SetColor(parameters.renderUnit.colorHex);

            WordIndex = parameters.wordIndex;

            if(!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public void TriggerPointerSelection() 
        {
            OnWordSelection?.Invoke(this);
        }

        public void SetSelected() 
        {
            OnSelected.Invoke();
        }

        public void SetDeselected() 
        {
            OnDeselected.Invoke();
        }

        private void Awake()
        {
            OnDeselected.Invoke();
        }

        private void OnDestroy()
        {
            OnWordSelection = null;
        }

        private void SetRightSpaces()
        {
            RightSpaces = 0;
            string text = textField.text;
            for(int i = text.Length - 1; i >= 0; i--)
            {
                if(text[i] != ' ') break;
                RightSpaces++;
            }
        }

        private void SetSize(out Vector2 size) 
        {
            size = textField.GetPreferredValues();

            highlighter.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);

            // Add space size
            size.x += textField.fontSize * fontSizeSpaceMul * RightSpaces;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        private void SetColor(string colorHex) 
        {
            if(colorHex != null)
            {
                if(ColorUtility.TryParseHtmlString(colorHex, out Color color))
                {
                    textField.color = color;

                    Color c = color;
                    c.a = highlighter.color.a;
                    highlighter.color = c;
                }
                else
                    throw new Exception("Invalid color hex: " + colorHex);
            }
        }

        private void OnValidate()
        {
            rectTransform = transform as RectTransform;
        }
    }
}