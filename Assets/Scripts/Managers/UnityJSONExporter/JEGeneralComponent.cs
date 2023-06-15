// Copyright (c) 2021, Wilhelm Schnepp
// Licensed under the MIT license, see LICENSE for details

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;


namespace JSONExporter
{

    public class JEGeneralComponent : JEComponent
    {
        override public void Preprocess()
        {
            

        }

        override public void QueryResources()
        {
            
        }

        new public static void Reset()
        {

        }

        public override JSONComponent ToJSON()
        {            
            var json = new JSONComponent();
            json.type = unityComponent.GetType().Name;

            return json;
        }
    }

}
