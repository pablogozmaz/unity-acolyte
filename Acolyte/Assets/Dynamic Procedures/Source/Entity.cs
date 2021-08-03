using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Depiction of a basic gameobject as required to exist in a Dynamic Procedure.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Properties))]
    [AddComponentMenu("Dynamic Procedures/Entity")]
    public class Entity : MonoBehaviour
    {
        public event Action OnDestruction;

        public string id;

        public Properties Properties => properties;

        [SerializeField, HideInInspector]
        private Properties properties;


        protected virtual void OnDestroy() 
        {
            OnDestruction?.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            properties = GetComponent<Properties>();

            Debug.Assert(properties != null);

            if(string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
        }
#endif
    }
}