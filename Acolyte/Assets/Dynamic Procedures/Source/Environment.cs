using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// The root entity containing all the available entities that may be acted upon during a procedure execution.
    /// </summary>
    [AddComponentMenu("Dynamic Procedures/Environment")]
    public class Environment : Entity
    {
        public static event Action<Environment> OnEnvironmentInitialized;

        public IEnumerable<Entity> AllEntities { get { return entityDictionary.Values; } }

        public AcolyteGameObjectsContainer AcolyteObjectsContainer { get { return acolyteObjectsContainer; } }

        private Dictionary<string, Entity> entityDictionary;

        [SerializeField]
        private AcolyteGameObjectsContainer acolyteObjectsContainer;


        public bool TryGetEntity(string id, out Entity entity)
        {
            return entityDictionary.TryGetValue(id, out entity);
        }

        public void SetEntities(Entity[] entityComponents)
        {
            entityDictionary = new Dictionary<string, Entity>(entityComponents.Length);

            for(int i = 0; i < entityComponents.Length; i++)
            {
                if(entityComponents[i] == this) continue;
                
                Debug.Assert(entityComponents[i] != null);

                string id = entityComponents[i].id;

                if(string.IsNullOrEmpty(id))
                {
                    Debug.LogWarning("Entity for Object " + entityComponents[i].name + " was discarded. Null or empty id.");
                    continue;
                }

                entityDictionary.Add(id, entityComponents[i]);
            }

            OnEnvironmentInitialized?.Invoke(this);
        }
    }
}