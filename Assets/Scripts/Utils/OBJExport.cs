using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
 
public class ExportMeshToOBJ
{
    public string ExportToOBJ(GameObject obj)
    {
        if (obj == null)
        {
            Debug.Log("No object selected.");
        }
 
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.Log("No mesh found in selected GameObject.");
        }
 

        Mesh mesh = meshFilter.sharedMesh;
        StringBuilder sb = new StringBuilder();
 
        foreach(Vector3 v in mesh.vertices)
        {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        foreach(Vector3 v in mesh.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        for (int material=0; material < mesh.subMeshCount; material++)
        {
            sb.Append(string.Format("\ng {0}\n", obj.name));
            int[] triangles = mesh.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0} {1}/{1} {2}/{2}\n",
                triangles[i] + 1,
                triangles[i + 1] + 1,
                triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }

    public Mesh OBJtoMesh(string obj){
        Mesh mesh = new Mesh();
        string[] lines = obj.Split('\n');
        ArrayList vertices = new ArrayList();
        ArrayList normals = new ArrayList();
        ArrayList faces = new ArrayList();
        foreach(string line in lines){
            string[] tokens = line.Split(' ');
            if(tokens[0] == "v"){
                Vector3 v = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                vertices.Add(v);
            }
            if(tokens[0] == "vn"){
                Vector3 v = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                normals.Add(v);
            }
            if(tokens[0] == "f"){
                int[] f = new int[3];
                f[0] = int.Parse(tokens[1].Split('/')[0]);
                f[1] = int.Parse(tokens[2].Split('/')[0]);
                f[2] = int.Parse(tokens[3].Split('/')[0]);
                faces.Add(f);
            }
        }
        mesh.vertices = (Vector3[])vertices.ToArray(typeof(Vector3));
        mesh.normals = (Vector3[])normals.ToArray(typeof(Vector3));
        mesh.triangles = (int[])faces.ToArray(typeof(int));
        return mesh;
    }

}

