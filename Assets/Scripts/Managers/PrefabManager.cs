using UnityEngine;
using System.Collections;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;
    public GameObject[] objects;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

}

