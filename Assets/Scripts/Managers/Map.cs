using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dummiesman;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using QFSW.QC;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;

public class Map : MonoBehaviour
{
    public static Map instance;
    public Dictionary<string, SavableEntity> objects;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        objects = new Dictionary<string, SavableEntity> {};
    }
    private void Start()
    {
        
    }

    private string toJson(){
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(objects, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });
        return json;
    }
    private void fromJson(string json)
    {
        objects = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SavableEntity>>(json);
    }
    [Command]
    private async Task SaveMap()
    {
        //Convert objects to json string
        string json = toJson();
        string cid = "";
        //Save to local file
        //Save to IPFS
        var temp = await ThirdwebManager.Instance.SDK.storage.UploadText(json);
        cid = temp.IpfsHash;
        //Save CID to PlayerPrefs
        PlayerPrefs.SetString("CID", cid);
        StoreNetworking.instance.UpdateLobby(cid);
    }
    private void LoadLocalMap()
    {
        //Load from local file
    }
    public async Task GenerateMap(string cid)
    {
        if (cid == "Don'tLoad") { return; }
        await LoadNetworkMap(cid);
        foreach (SavableEntity e in objects.Values)
        {
            GameObject a = convertFromSavable(e);
            GameObject o = Instantiate(a, e.pos, e.rotation);
            o.name = e.Id;
            o.layer = 3;
            Debug.Log(o.name);
        }
    }
    private async Task LoadNetworkMap(string cid)
    {
        //Load from IPFS with Lobby CID & Serialize to objects
        string ipfsLink = ThirdwebManager.Instance.storageIpfsGatewayUrl;
        objects = await ThirdwebManager.Instance.SDK.storage.DownloadText<Dictionary<string,SavableEntity>>(ipfsLink + cid);
    }

    public GameObject convertFromSavable(SavableEntity e)
    {
        // ExportMeshToOBJ exporter = new ExportMeshToOBJ();
        GameObject o = new GameObject();
        o.name = e.Id;
        o.transform.SetPositionAndRotation(e.pos, e.rotation);
        o.transform.localScale = e.scale;
        MeshFilter filter = o.AddComponent<MeshFilter>();
        //Convert string to mesh
        Debug.Log(e.mesh);
        // Mesh m = exporter.OBJtoMesh(e.mesh);
        // filter.mesh = m;
        MeshRenderer render = o.AddComponent<MeshRenderer>();
        o.AddComponent<MeshCollider>();
        o.layer = 3;
        return o;
    }
    public void convertToSavable(GameObject o)
    {   
        // ExportMeshToOBJ exporter = new ExportMeshToOBJ();
        // string mesh = exporter.ExportToOBJ(o);
        // Debug.Log(mesh);
        // return new SavableEntity(
        //      o.name.Length < 36 ? Guid.NewGuid().ToString() : o.name.Substring(0, 36),
        //        o.transform.position,
        //        o.transform.rotation,
        //        o.transform.localScale,
        //        mesh
        //     );
    }
    [Command]
    public void ImportOBJ(string filePath)
    {
        //file path
        filePath = "/Users/tarrenhassman/Downloads/textured_mesh_medpoly_obj";
        if (!File.Exists(filePath))
        {
            Debug.LogError("Please set FilePath in ObjFromFile.cs to a valid path.");
            return;
        }

        //load
        GameObject o = new OBJLoader().Load(filePath);
        o.AddComponent<MeshRenderer>();
        Instantiate(o);
    }
}


public class ReferenceManager
{

}

public class UiManager
{

}