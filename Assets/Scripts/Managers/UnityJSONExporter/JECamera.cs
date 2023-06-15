// Copyright (c) 2014-2015, THUNDERBEAST GAMES LLC
// Licensed under the MIT license, see LICENSE for details

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

namespace JSONExporter
{

public class JECamera : JEComponent
{
    new public static void Reset()
    {

    }

    public override JSONComponent ToJSON()
    {
        var cam = unityComponent as Camera;
        var json = new JSONCamera();
        json.type = "Camera";
        json.projection = cam.orthographic ? ProjectionType.Orthogonal : ProjectionType.Perspective;
        json.fovVertical = cam.fieldOfView;
        json.near = cam.nearClipPlane;
        json.far = cam.farClipPlane;
        json.aspectRatio = cam.aspect;
        return json;
    }

}

}
