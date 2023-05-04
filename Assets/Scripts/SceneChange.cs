using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public int sceneBuildIndex;

  protected void  OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Player"){
            ChangeScene();
        }
    }
    public void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex);
    }
}

enum Scenes {
    menu = 0,
    customer = 1,
    merchant = 2,
}
