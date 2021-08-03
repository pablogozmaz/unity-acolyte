using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    [CreateAssetMenu(fileName = "Diesel Editor Configuration", menuName = "Diesel/Editor Configuration")]
    public class AcolyteEditorConfiguration : ScriptableObject
    {
        public TMP_FontAsset FontAsset { get { return fontAsset; } }

        [SerializeField]
        private TMP_FontAsset fontAsset;
    }
}