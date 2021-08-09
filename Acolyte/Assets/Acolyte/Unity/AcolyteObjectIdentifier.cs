using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.UnityEngine
{
    public class AcolyteObjectIdentifier : MonoBehaviour
    {
        public string ID { get { return id; } }

        [SerializeField]
        private string id;

        [SerializeField]
        private string[] tags;


        public bool ContainsTag(string tag)
        {
            for(int i = 0; i < tags.Length; i++)
            {
                if(tags[i] == tag) return true;
            }

            return false;
        }
    }
}