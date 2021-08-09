using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.UnityEngine
{
    [ExecuteInEditMode]
    public class AcolyteObjectsContext : MonoBehaviour, IExecutionSubcontext
    {
        [SerializeField]
        private AcolyteObjectIdentifier[] identifiers = new AcolyteObjectIdentifier[0];


        public GameObject TryGetObject(string id, string[] tags = null)
        {
            for(int i = 0; i < identifiers.Length; i++)
            {
                if(identifiers[i].ID == id)
                {
                    if(tags == null || ContainsTags(identifiers[i], tags))
                    {
                        return identifiers[i].gameObject;
                    }
                }
            }

            return null;
        }

        private bool ContainsTags(AcolyteObjectIdentifier identifier, string[] tags)
        {
            int tagsPassed = 0;

            for(int t = 0; t < tags.Length; t++)
            {
                if(identifier.ContainsTag(tags[t]))
                    tagsPassed++;
            }

            return tagsPassed == tags.Length;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(!Application.isPlaying)
                identifiers = GetComponentsInChildren<AcolyteObjectIdentifier>();
        }
#endif
    }
}