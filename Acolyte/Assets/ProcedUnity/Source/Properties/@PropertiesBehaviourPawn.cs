using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    /// <summary>
    /// Abstract behaviour whose logic requires a method call from an external source.
    /// </summary>
    public abstract class PropertiesBehaviourPawn<T> : PropertiesBehaviour<T> where T : IProperty
    {
        public void ProcessBehaviour() 
        {
            ProcessBehaviour(propertiesTracker.GetTrackedProperties);
        }

        protected sealed override void Begin() {}
        protected sealed override void Stop () {}

        protected abstract void ProcessBehaviour(IEnumerable<T> properties);
    }
}