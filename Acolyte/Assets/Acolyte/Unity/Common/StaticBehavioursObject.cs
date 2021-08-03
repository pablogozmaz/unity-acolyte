using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte
{
    public static class StaticBehavioursObject
    {
        private static GameObject gameObject;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateGameObject()
        {
            if(gameObject != null) return;

            gameObject = new GameObject("Acolyte static behaviours");
            Object.DontDestroyOnLoad(gameObject);
        }

        public static T AddComponent<T>() where T : MonoBehaviour
        {
            Debug.Assert(gameObject != null, "Static Behaviour Object should not be called before scene has loaded.");

            return gameObject.AddComponent<T>();
        }
    }
}