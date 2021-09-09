using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte.Editor
{
    /// <summary>
    /// Contains configuration variables for script rendering.
    /// </summary>
    [Serializable]
    public class RenderConfiguration
    {
        public static RenderConfiguration Current { get; private set; }

        public string commentColor = "#A3A3A3";
        public string keywordColor = "#51A3F5";
        public string statementColor = "#51A3F5";
        public string identifierColor = "#CCA88E";
        public string literalColor = "#CCA88E";
        public string validUndefinedColor = "#FFFFFF";
        public string toleratedColor = "#FFFFFF";
        public string unrecognizedColor = "#CC1F37";


        static RenderConfiguration() 
        {
            // Always set default values at start
            Current = new RenderConfiguration();
        }

        public static void SetCurrent(RenderConfiguration config)
        {
            if(config == null)
                return;

            Current = config;
        }
    }
}