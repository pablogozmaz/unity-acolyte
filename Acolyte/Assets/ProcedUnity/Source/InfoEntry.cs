using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedUnity
{
    /// <summary>
    /// Contains user-friendly data values as an information entry that may help describe an object or a process.
    /// </summary>
    [Serializable]
    public struct InfoEntry
    {
        public string header;
        public string body;
    }
}