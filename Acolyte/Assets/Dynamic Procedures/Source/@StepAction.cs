using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    public abstract class StepAction : ScriptableObject
    {
        public abstract string TypeLabel { get; }

        public abstract bool HasDefaultEntityList { get; }

        public List<string> entities;
    }
}