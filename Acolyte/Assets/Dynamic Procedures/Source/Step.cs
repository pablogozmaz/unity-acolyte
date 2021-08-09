using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    [Serializable]
    public struct Step
    {
        public string name;

        public List<InfoEntry> infoEntries;

        public List<StepAction> actions;

        public Acolyte.Script Script
        {
            get
            {
                if(script == null)
                    script = new Acolyte.Script(scriptAsset.text, StepScript.instance, true);
                return script;
            }
        }

        private Acolyte.Script script;

        [SerializeField]
        private TextAsset scriptAsset;
    }
}