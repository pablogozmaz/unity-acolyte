using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace TFM.DynamicProcedures.Examples
{
    public class JsonExamples
    {
        public static string Serialize(Procedure procedure)
        {
            // Manual serialization is requried
            JObject jsonObject = new JObject
            {
                { "name", procedure.name }
            };

            JArray jsonSteps = new JArray();
            for(int i = 0; i < procedure.steps.Count; i++)
            {
                jsonSteps.Add(CreateStepJObject(procedure.steps[i]));
            }

            jsonObject.Add("steps", jsonSteps);

            string json = jsonObject.ToString(Formatting.Indented);
            Debug.Log("Procedure json example:\n" + json);

            return json;
        }

        public static Procedure Deserialize(string json) 
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var procedure = JsonConvert.DeserializeObject<Procedure>(json, settings);

            Debug.Log("Procedure json deserialization:  " + procedure);

            return procedure;
        }

        private static JObject CreateStepJObject(Step step)
        {
            JsonSerializer serializer = new JsonSerializer()
            {
                ContractResolver = new ExampleContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Required because Unity reasons
            };

            JArray actionsArray = JArray.FromObject(step.actions, serializer);

            return new JObject
            {
                { "name",    step.name },
                { "actions", actionsArray }
            };
        }

        private class ExampleContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                if(member.Name == "hideFlags") return null;

                var prop = base.CreateProperty(member, memberSerialization);

                return prop.Writable ? prop : null;
            }
        }
    }
}