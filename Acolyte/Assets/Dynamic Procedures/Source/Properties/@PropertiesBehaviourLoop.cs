using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Abstract behaviour that processes logic in a loop upon one or more properties at a specific frequency.
    /// </summary>
    public abstract class PropertiesBehaviourLoop<T> : PropertiesBehaviour<T> where T : IProperty
    {
        protected abstract float FrequencyInSeconds { get; }

        private IEnumerator loopCoroutine;


        protected sealed override void Begin()
        {
            Debug.Assert(loopCoroutine == null);

            loopCoroutine = CoroutineProcess(new WaitForSecondsRealtime(FrequencyInSeconds));
            StaticCoroutineRunner.Start(loopCoroutine);
        }

        protected sealed override void Stop()
        {
            if(loopCoroutine != null)
            {
                StaticCoroutineRunner.Stop(loopCoroutine);
                loopCoroutine = null;
            }
        }

        protected abstract void ProcessBehaviour(IEnumerable<T> properties);

        private IEnumerator CoroutineProcess(WaitForSecondsRealtime wait) 
        {
            yield return null;

            while(true)
            {
                ProcessBehaviour(propertiesTracker.GetTrackedProperties);

                yield return wait;
            }
        }
    }
}