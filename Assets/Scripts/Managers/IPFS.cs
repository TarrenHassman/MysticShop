using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONExporter;
using QFSW.QC;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class IPFS : MonoBehaviour
{
   Dictionary<string, JSONScene> Scenes;
   IPFS Instance;


    public void Awake()
    {
        Scenes = new Dictionary<string, JSONScene>();
        Instance = this;
        DontDestroyOnLoad(this);
    }
   
    [Command]
    public async Task SaveScene(string sceneName)
    {
        Debug.Log("Saving Scene");
        string path = "Assets/Scenes/" + sceneName + ".txt";
        List<string> ignoredTags = new List<string>();
        ignoredTags.AddRange(new string[] { "Player", "MainCamera", "CinemachineTarget", "Ground" });
        Debug.Log("Ignored Tags: " + ignoredTags.Count);
        UnityJSONExporter.RegisterCallback callback = new UnityJSONExporter.RegisterCallback(() => { });
        Debug.Log("callback");
        // var jsonScene = UnityJSONExporter.GenerateJSONScene(ignoredTags, false, false, true, callback);
        var jsonScene = UnityJSONExporter.GenerateJSONScene(ignoredTags, false, false, true, callback);
        Debug.Log("jsonScene");
        JsonConverter[] converters = new JsonConverter[] { new BasicTypeConverter() };
        Debug.Log("converters");
        string json = JsonConvert.SerializeObject(jsonScene, Formatting.Indented, converters);
        string cid = "";
        //Save to local file
        System.IO.File.WriteAllText(path, json);
        //Save to IPFS
        Debug.Log("Saving to IPFS");
        var temp = await ThirdwebManager.Instance.SDK.storage.UploadText(json);
        cid = temp.IpfsHash;
        //Save CID to PlayerPrefs
        PlayerPrefs.SetString("CID", cid);
        StoreNetworking.instance.UpdateLobby(cid);
    }

    [Command]
    public async Task LoadScene(string? cid)
    {
        cid = cid ?? (PlayerPrefs.HasKey("CID") ? PlayerPrefs.GetString("CID") : "Don'tLoad");
        //Load from local file
        //Load from IPFS
        JSONScene temp = await ThirdwebManager.Instance.SDK.storage.DownloadText<JSONScene>(cid!);
        GenerateScene(temp);
        // UnityJSONImporter.ImportJSONScene(jsonScene, null);
    }
    public void GenerateScene(JSONScene scene){
       scene.hierarchy.ForEach(o=>TraverseObjects(o));
       DebugResources(scene.resources);
    }
    public void TraverseObjects(JSONGameObject obj){
        GameObject go = new GameObject(obj.name);
        // go.transform.position = obj.position;
        // go.transform.rotation = obj.rotation;
        // go.transform.localScale = obj.scale;
        Debug.Log(obj.name);
        obj.components.ForEach(c=>Debug.Log(c.type));
        obj.children.ForEach(o=>TraverseObjects(o));
    }
    public void DebugResources(JSONResources res){
        res.textures.ForEach(t=>Debug.Log(t.name));
        res.lightmaps.ForEach(l=>Debug.Log(l.filename));
        res.shaders.ForEach(s=>Debug.Log(s.name));
        res.materials.ForEach(m=>Debug.Log(m.name));
        res.meshes.ForEach(m=>Debug.Log(m.name));
    }
    //Write to file?
}

//TODO:
//Tag managers/controllers and add to ignore tags
//IPFS CID to PlayerPrefs
//IPFS CID to Lobby
//Generate Scene from JSONScene