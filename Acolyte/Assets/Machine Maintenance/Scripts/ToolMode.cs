using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    public enum ToolModes
    {
        manualMode,
        electricMode
    }

    public class ToolMode : MonoBehaviour
    {
        public static ToolModes Mode { get; private set; } = ToolModes.manualMode;


        public void SetModeManual() 
        {
            Mode = ToolModes.manualMode;
        }

        public void SetModeElectric()
        {
            Mode = ToolModes.electricMode;
        }
    }
}