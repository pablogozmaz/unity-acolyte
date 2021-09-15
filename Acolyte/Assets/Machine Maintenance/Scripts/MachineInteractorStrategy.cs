using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity.Examples
{
    [CreateAssetMenu(fileName = "Machine Interaction Strategy", menuName = "Machine Interaction Strategy")]
    public class MachineInteractorStrategy : Interactor.Strategy
    {
        public override void HandlePropertyValue(Interactor interactor, InteractionState value)
        {
            Animator aniamtor = interactor.GetComponent<Animator>();

            if(aniamtor != null)
            {
                string trigger;

                switch(value.Value)
                {
                    case InteractionStates.NonInteractable:
                    default:
                        trigger = "Default";
                        break;

                    case InteractionStates.Interactable:
                        trigger = "Pending";
                        break;

                    case InteractionStates.Interacted:
                        trigger = "Completed";
                        break;

                    case InteractionStates.InteractedIncorrect:
                        trigger = "Incorrect";
                        break;
                }

                aniamtor.SetTrigger(trigger);
            }
        }
    }
}