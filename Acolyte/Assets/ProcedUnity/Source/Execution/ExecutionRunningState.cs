using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedUnity
{
    public enum ExecutionRunningState
    {
        AwaitingStart,
        InProcess,
        Completed,
        Cancelled
    }
}