using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Abstract behaviour that processes logic as a response to a property's value change.
    /// </summary>
    public abstract class PropertiesBehaviourValueObserver<T> : PropertiesBehaviour<T> where T : IProperty
    {
        private bool isProcessing;


        protected sealed override void Begin()
        {
            propertiesTracker.OnPropertyFound += TryEvaluateValueChange;
            propertiesTracker.OnPropertyValueChanged += TryEvaluateValueChange;
        }

        protected sealed override void Stop()
        {
            propertiesTracker.OnPropertyFound -= TryEvaluateValueChange;
            propertiesTracker.OnPropertyValueChanged -= TryEvaluateValueChange;
        }

        private void TryEvaluateValueChange(T property) 
        {
            if(isProcessing) return;

            isProcessing = true;

            try
            {
                ProcessValueChange(property);
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message+"\n"+ex.StackTrace);
            }

            isProcessing = false;
        }

        protected abstract void ProcessValueChange(T property);
    }
}