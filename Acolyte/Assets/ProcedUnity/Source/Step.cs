using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    [Serializable]
    public struct Step
    {
        public string name;

        public List<InfoEntry> infoEntries;

        public List<StepAction> actions;

        public Acolyte.Script Script => scriptAsset.Script;

        [SerializeField]
        private StepScriptAsset scriptAsset;
    }
}