// Copyright (c) 2014-2015, THUNDERBEAST GAMES LLC
// Licensed under the MIT license, see LICENSE for details

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

namespace JSONExporter
{

public class JEScene : JEObject
{

    public static string sceneName;

    public static JEScene TraverseScene(List<string> ignoreTags, bool disabledGOs, bool disabledComponents, bool includeUnknown)
    {
        var scene = new JEScene();

        List<GameObject> root = new List<GameObject>();

        // Unity has no root object, so collect root game objects this way
        object[] objects = GameObject.FindObjectsOfType(typeof (GameObject));

        foreach (object o in objects)
        {
            GameObject go = (GameObject) o;
            if (ignoreTags.Contains(go.tag))
                continue;
            if (go.transform.parent == null)
                root.Add(go);
        }

        if (root.Count == 0)
        {
            ExportError.FatalError("Cannot Export Empty Scene");
        }

        // traverse the "root" game objects, collecting child game objects and components
        foreach (var go in root)
        {
            var rgo = Traverse(go, ignoreTags, disabledGOs, disabledComponents, includeUnknown);
            if(rgo != null)
                scene.rootGameObjects.Add(rgo);
        }

        return scene;
    }

    public void Preprocess()
    {
        foreach(var jgo in rootGameObjects)
            jgo.Preprocess();
        // discover resources
        foreach(var jgo in rootGameObjects)
            jgo.QueryResources();

        JEResource.Preprocess();

    }

    public void Process()
    {
        JEResource.Process();
        foreach(var jgo in rootGameObjects)
            jgo.Process();
    }

    public void PostProcess()
    {
        JEResource.PostProcess();

        foreach(var jgo in rootGameObjects)
            jgo.PostProcess();
    }

    public static void Reset()
    {

    }

    List<JEGameObject> rootGameObjects = new List<JEGameObject>();

    static JEGameObject Traverse(GameObject obj, List<string> ignoreTags, bool disabledGOs, bool disabledComponents, bool includeUnknown, JEGameObject jparent = null)
    {
        if ((!disabledGOs && !obj.activeSelf) || ignoreTags.Contains(obj.tag))
            return null;
        JEGameObject jgo = new JEGameObject(obj, jparent, disabledComponents, includeUnknown);

        foreach (Transform child in obj.transform)
        {
            Traverse(child.gameObject, ignoreTags, disabledGOs, disabledComponents, includeUnknown, jgo);
        }

        return jgo;
    }

    public new JSONScene ToJSON()
    {
        var json = new JSONScene();

        json.name = sceneName;

        json.resources = JEResource.GenerateJSONResources();

        json.hierarchy = new List<JSONGameObject>();
        foreach (var go in rootGameObjects)
            json.hierarchy.Add(go.ToJSON());

        return json;

    }
}

}
