using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    [CreateAssetMenu(fileName = "Procedure", menuName = "Dynamic Procedures/" + "Procedure")]
    public class Procedure : ScriptableObject
    {
        public List<InfoEntry> infoEntries;

        public List<Step> steps = new List<Step>();
    }
}