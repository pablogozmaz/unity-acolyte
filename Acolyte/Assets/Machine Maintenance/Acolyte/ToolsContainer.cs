using System.Collections;
using System.Collections.Generic;


namespace ProcedUnity
{
    public class ToolsContainer : Acolyte.UnityContainerBehaviour<Tool>
    {
        public static ToolsContainer Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            if(Instance == this)
                Instance = null;
        }
    }
}