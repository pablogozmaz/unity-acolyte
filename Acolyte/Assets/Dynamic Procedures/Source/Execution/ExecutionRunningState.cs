using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFM.DynamicProcedures
{
    public enum ExecutionRunningState
    {
        AwaitingStart,
        InProcess,
        Completed,
        Cancelled
    }
}