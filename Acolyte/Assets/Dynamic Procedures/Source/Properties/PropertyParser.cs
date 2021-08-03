using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertyFactory = System.Func<string, string, TFM.DynamicProcedures.IProperty>;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Provides methods for appending properties by parsing.
    /// </summary>
    public static class PropertyParser
    {
        private struct Parse
        {
            public string type;
            public string name;
            public string value;
        }

        private static readonly Dictionary<string, PropertyFactory> propertyFactories = new Dictionary<string, PropertyFactory>
        {
            { "bool",   (string name, string value) => { return  new PropertyBool  (name, value); } },
            { "int",    (string name, string value) => { return  new PropertyInt   (name, value); } },
            { "float",  (string name, string value) => { return  new PropertyFloat (name, value); } },
            { "string", (string name, string value) => { return  new PropertyString(name, value); } }
        };


        /// <summary>
        /// Adds a factory method for the creation of a custom property type, to be used with the given type prefix.
        /// </summary>
        public static void AddCustomFactory(string typePrefix, PropertyFactory customFactory) 
        {
            Debug.Assert(customFactory != null);

            if(!propertyFactories.ContainsKey(typePrefix))
                propertyFactories.Add(typePrefix, customFactory);
            else
                Debug.Assert(false, "Cannot add new custom factory method for prefix already present: "+typePrefix);
        }

        /// <summary>
        /// Parse the given text line by line, appending all found properties.
        /// </summary>
        public static void ParseAndAppend(string text, Properties properties)
        {
            StringReader strReader = new StringReader(text);
            string line = strReader.ReadLine();

            while(line != null)
            {
                try
                {
                    line = line.Trim();
                    if (line.Length > 0)
                    {
                        if (TryParseLine(line, out Parse parse))
                        {
                            AppendParseToProperties(parse, properties);
                        }
                    }

                    // Iterate to next line
                    line = strReader.ReadLine();
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private static bool TryParseLine(string line, out Parse parse)
        {
            Debug.Assert(!string.IsNullOrEmpty(line));

            parse = new Parse
            {
                type = FindTypePrefix(line, out int traverseIndex)
            };

            if(string.IsNullOrEmpty(parse.type))
            {
                Debug.Assert(false, "Parse error. Prefix not found in: <b>" + line + "</b>");
                return false;
            }

            parse.name = FindName(line, ref traverseIndex);

            if(string.IsNullOrEmpty(parse.name))
            {
                Debug.Assert(false, "Parse error. Name not found in: <b>" + line+"</b>");
                return false;
            }

            traverseIndex++;
            parse.value = FindValue(line, traverseIndex);

            if(string.IsNullOrEmpty(parse.value))
            {
                Debug.Assert(false, "Parse error. Value not found in: <b>" + line + "</b>");
                return false;
            }

            return true;
        }

        private static void AppendParseToProperties(Parse parse, Properties properties)
        {
            if(propertyFactories.TryGetValue(parse.type, out var factoryMethod))
            {
                var property = factoryMethod.Invoke(parse.name, parse.value);

                if(property != null)
                    properties.Add(property);
            }
        }

        private static string FindTypePrefix(string line, out int traverseIndex) 
        {
            for(traverseIndex = 0; traverseIndex < line.Length; traverseIndex++)
            {
                if(line[traverseIndex] == ' ')
                {
                    return line.Substring(0, traverseIndex);
                }
            }

            return null;
        }

        private static string FindName(string line, ref int traverseIndex) 
        {
            int indexStart = traverseIndex;

            for(; traverseIndex < line.Length; traverseIndex++)
            {
                if(line[traverseIndex] == '=')
                {
                    return line.Substring(indexStart, traverseIndex - 1 - indexStart).Trim();
                }
            }

            return null;
        }

        private static string FindValue(string line, int traverseIndex) 
        {
            int lengthLeft = line.Length - traverseIndex;

            return line.Substring(traverseIndex, lengthLeft).Trim();
        }
    }
}