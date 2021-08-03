using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Interface used to identify Execution objects.
    /// </summary>
    public interface IExecution
    {
        string Label { get; }
    }
}