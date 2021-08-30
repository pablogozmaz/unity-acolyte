using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


namespace Acolyte.Editor
{
    public struct WordRenderParameters
    {
        public RenderUnit renderUnit;
        public Vector2 size;
        public Vector2 localPosition;
        public int lineIndex;
        public int wordIndexInLine;
        public int charIndexMin;
        public int charIndexMax;
    }

    [RequireComponent(typeof(RectTransform))]
    public class AcolyteWordRenderer : MonoBehaviour, IPointerEnterHandler
    {
        public RectTransform RectTransform => rectTransform;

        public string Text => textField.text;

        public int LineIndex { get; private set; }
        public int WordIndexInLine { get; private set; }

        public int CharIndexMin { get; private set; }
        public int CharIndexMax { get; private set; }

        [SerializeField, HideInInspector]
        private RectTransform rectTransform;

        [SerializeField]
        private TextMeshProUGUI textField;

        [SerializeField, HideInInspector]
        private Color defaultColor;


        public void Set(WordRenderParameters parameters)
        {
            if(!gameObject.activeSelf)
                gameObject.SetActive(true);

            textField.text = parameters.renderUnit.content;

            if(ColorUtility.TryParseHtmlString(parameters.renderUnit.colorHex, out Color color))
                textField.color = color;
            else
                textField.color = defaultColor;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parameters.size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parameters.size.y);
            rectTransform.localPosition = parameters.localPosition;

            LineIndex = parameters.lineIndex;
            WordIndexInLine = parameters.wordIndexInLine;
            CharIndexMin = parameters.charIndexMin;
            CharIndexMax = parameters.charIndexMax;
        }

        public void SetSelected() 
        {

        }

        public void SetDeselected()
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public bool IsCharIndexInRange(int charIndex)
        {
            return charIndex >= CharIndexMin && charIndex <= CharIndexMax;
        }

        private void OnValidate()
        {
            rectTransform = transform as RectTransform;

            if(textField != null)
                defaultColor = textField.color;
        }
    }
}