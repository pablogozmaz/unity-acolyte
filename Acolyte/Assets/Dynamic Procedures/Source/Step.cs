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
    }
}