using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableEntity
{
    public SavableEntity(
        string id,
        Vector3 pos, Quaternion rotation, Vector3 scale,
        string mesh
        ) {
        this.id = id;
        this.pos = pos;
        this.rotation = rotation;
        this.scale = scale;
        this.mesh = mesh;
    }

    private string id;
    public string Id => id;
    public Vector3 pos;
    public Quaternion rotation;
    public Vector3 scale;
    public string mesh;
    public MeshRenderer renderer;
}


