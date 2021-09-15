using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ProcedUnity
{
    public class Tool : MonoBehaviour
    {
        public static event Action OnSelection;

        public static Tool Current { get; protected set; }

        [SerializeField]
        private UnityEvent OnSelected;

        [SerializeField]
        private UnityEvent OnDeselected;


        public static void ClearSelection()
        {
            if(Current != null)
            {
                Current.Deselect();

                Current = null;

                OnSelection?.Invoke();
            }
        }

        public virtual void Select()
        {
            if(Current != null)
            {
                Current.Deselect();
            }

            Current = this;

            OnSelected.Invoke();
            OnSelection?.Invoke();
        }

        public void Deselect()
        {
            if(Current == this)
            {
                Current = null;

                OnDeselected.Invoke();
            }
        }
    }
}