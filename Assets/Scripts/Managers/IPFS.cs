using System.Collections.Generic;
using UnityEngine;
using JSONExporter;
using QFSW.QC;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class IPFS : MonoBehaviour
{
   Dictionary<string, JSONScene> Scenes;
   public static IPFS Instance;

   public void Awake()
    {
        Scenes = new Dictionary<string, JSONScene>();
        Instance = this;
        DontDestroyOnLoad(this);
    }
   
#nullable enable
    //Improve performance by using Jobs to upload each gameobject to IPFS through a job and then add the cids to a main file
    [Command]
    public async Task SaveScene(string sceneName)
    {
        string path = "Assets/Scenes/" + sceneName + ".json";
        List<string> ignoredTags = new List<string>();
        ignoredTags.AddRange(new string[] { "Player", "MainCamera", "CinemachineTarget", "Ground", "Manager" });
        UnityJSONExporter.RegisterCallback callback = new UnityJSONExporter.RegisterCallback(() => { });
        var jsonScene = UnityJSONExporter.GenerateJSONScene(ignoredTags, false, false, true, callback);
        JsonConverter[] converters = new JsonConverter[] { new BasicTypeConverter() };
        string json = JsonConvert.SerializeObject(jsonScene, Formatting.Indented, converters);
        string? cid = null;
        //Gzip data
        json = StringCompressor.CompressString(json);
        //Save to local file
        System.IO.File.WriteAllText(path, json);
        //Save to IPFS
        var temp = await ThirdwebManager.Instance.SDK.storage.UploadText(json);
        cid = temp.IpfsHash;
        //Save CID to PlayerPrefs
        PlayerPrefs.SetString("CID", cid);
        StoreNetworking.instance.UpdateLobby(cid);
    }

    [Command]
    public async Task LoadScene(string? cid)
    {
        //Load from local file
        // string sceneName = "example";
        // string path = "Assets/Scenes/" + sceneName + ".json";
        // if (System.IO.File.Exists(path))
        // {
        //     string json = System.IO.File.ReadAllText(path);
        //     JSONScene temp = JsonConvert.DeserializeObject<JSONScene>(json);
        //     GenerateScene(temp);
        // }
        // else
        // {
            //Load from IPFS
            if (cid != null)
            {
                string compressed = await ThirdwebManager.Instance.SDK.storage.DownloadText<string>(cid!);
                JSONScene? temp = JsonConvert.DeserializeObject<JSONScene>(StringCompressor.DecompressString(compressed));
                GenerateScene(temp);
            }   
        // }
    }
    private void GenerateScene(JSONScene? scene)
    {
        if (scene != null)
        {
            Debug.Log(scene.name);
            scene.hierarchy.ForEach(o => TraverseObject(o, null));
            DebugResources(scene.resources);
        }
    }
    public void TraverseObject(JSONGameObject obj, GameObject? parent){
        GameObject go = new GameObject(obj.name);
        if (parent != null)
        {
            go.transform.SetParent(parent.transform);
        }
        go.transform.position = obj.GetComponent<JSONTransform>().localPosition;
        go.transform.rotation = obj.GetComponent<JSONTransform>().localRotation;
        go.transform.localScale = obj.GetComponent<JSONTransform>().localScale;
        obj.components.ForEach(c=>Debug.Log(c.type));
        obj.children.ForEach(o=>
        {
            TraverseObject(o, go);
        });
    }
#nullable disable
    public void DebugResources(JSONResources res){
        res.textures.ForEach(t=>Debug.Log(t.name));
        res.lightmaps.ForEach(l=>Debug.Log(l.filename));
        res.shaders.ForEach(s=>Debug.Log(s.name));
        res.materials.ForEach(m=>Debug.Log(m.name));
        res.meshes.ForEach(m=>Debug.Log(m.name));
    }
}

public class JsonImporter
{
    
}

//TODO:
//Generate Scene from JSONScene
