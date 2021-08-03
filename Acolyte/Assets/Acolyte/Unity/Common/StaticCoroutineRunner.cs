using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte
{
    /// <summary>
    /// Allows to statically start and stop coroutines through a gameobject that is always active.
    /// </summary>
    public static class StaticCoroutineRunner
    {
        // The MonoBehaviour attached to a GameObject that actually runs the coroutines.
        private class Behaviour : MonoBehaviour
        {
            private void Awake()
            {
                if(runner != null)
                {
                    Destroy(gameObject);
                    Debug.Assert(false);
                    return;
                }

                DontDestroyOnLoad(this);
            }

            private void OnDestroy()
            {
                runner.StopAllCoroutines();
                runner = null;
            }
        }

        private static Behaviour runner;

        private static HashSet<IEnumerator> pendingCoroutines = new HashSet<IEnumerator>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateGameObject()
        {
            if(runner != null) return;

            runner = StaticBehavioursObject.AddComponent<Behaviour>();
            
            // Start all pending coroutines, if any
            foreach(var pendingCoroutine in pendingCoroutines)
            {
                Start(pendingCoroutine);
            }

            pendingCoroutines = null;
        }

        public static void Start(IEnumerator coroutine)
        {
            if(runner == null)
            {
                if(pendingCoroutines != null) pendingCoroutines.Add(coroutine);
                return;
            }

            runner.StartCoroutine(coroutine);
        }

        public static void Stop(IEnumerator coroutine)
        {
            if(runner == null)
            {
                if(pendingCoroutines != null) pendingCoroutines.Remove(coroutine);
                return;
            }

            runner.StopCoroutine(coroutine);
        }
    }
}
