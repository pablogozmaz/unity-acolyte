using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte
{
    /// <summary>
    /// Allows invoking Actions on the next app thread's Update call.
    /// </summary>
    public class AppThreadInvoker
    {
        private class Behaviour : MonoBehaviour
        {
            private void Update()
            {
                InvokeAllActions();
            }
        }

        private static readonly ConcurrentQueue<Action> pendingActions = new ConcurrentQueue<Action>();


        public static void AddAction(Action action)
        {
            if(action == null || action.GetInvocationList() == null) return;

            pendingActions.Enqueue(action);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateBehaviour()
        {
            StaticBehavioursObject.AddComponent<Behaviour>();
        }

        private static void InvokeAllActions()
        {
            if(pendingActions.Count == 0) return;

            for(int i = 0; i < pendingActions.Count; i++)
            {
                if(pendingActions.TryDequeue(out Action action)) 
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
    }
}