using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TFM.DynamicProcedures.DebugUI
{
    public class EnvironmentView : EntityView
    {
        [SerializeField]
        private EntityView entityViewPrefab;

        private readonly List<EntityView> entityViews = new List<EntityView>();


        public void SetEnvironment(Environment environment) 
        {
            SetEntity(environment);

            int i = 0;
            foreach(var entity in environment.AllEntities)
            {
                if(i >= entityViews.Count) CreateEntityView();

                entityViews[i].SetEntity(entity);
                i++;
            }

            // Pool
            for(; i < entityViews.Count; i++)
            {
                entityViews[i].Activate(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        private void Awake()
        {
            entityViewPrefab.gameObject.SetActive(false);
        }

        private EntityView CreateEntityView() 
        {
            EntityView view = Instantiate(entityViewPrefab, entityViewPrefab.transform.parent);
            view.Activate(true);

            entityViews.Add(view);


            return view;
        }
    }
}