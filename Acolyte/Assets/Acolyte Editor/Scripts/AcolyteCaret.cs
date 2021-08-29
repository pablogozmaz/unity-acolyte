using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Acolyte.Editor
{
    [RequireComponent(typeof(Image))]
    public class AcolyteCaret : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Image image;

        [SerializeField]
        private float frequency = 0.5f;

        private float timer;


        private void OnEnable()
        {
            image.enabled = true;
            timer = 0;
        }

        private void Update()
        {
            timer += Time.unscaledDeltaTime;

            if(timer >= frequency)
            {
                image.enabled = !image.enabled;
                timer = 0;
            }
        }

        private void OnValidate()
        {
            image = GetComponent<Image>();
        }
    }
}