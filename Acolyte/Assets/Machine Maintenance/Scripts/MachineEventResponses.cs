using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    public class MachineEventResponses
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateEventResponses() 
        {
            new EventResponse("show_message", ()=> { Debug.Log("message"); });
        }
    }
}