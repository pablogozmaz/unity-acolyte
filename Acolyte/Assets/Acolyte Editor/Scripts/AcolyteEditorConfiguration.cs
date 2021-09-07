using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    [CreateAssetMenu(fileName = "Acolyte Editor Configuration", menuName = "Acolyte/Editor Configuration")]
    public class AcolyteEditorConfiguration : ScriptableObject
    {
        public TMP_FontAsset FontAsset { get { return fontAsset; } }

        [SerializeField]
        private TMP_FontAsset fontAsset;

        [SerializeField]
        private RenderConfiguration renderConfiguration;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() 
        {
            var array = Resources.LoadAll<AcolyteEditorConfiguration>("");

            if(array.Length == 0)
                return;

            var editorConfig = array[0];

            RenderConfiguration.SetCurrent(editorConfig.renderConfiguration);
        }
    }
}