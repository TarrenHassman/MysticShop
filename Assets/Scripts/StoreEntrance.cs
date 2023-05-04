using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreEntrance : MonoBehaviour
{
    public string lobbyCode;

    public int sceneBuildIndex = 0;

  protected void  OnTriggerEnter(Collider other)
    {
        Debug.Log(lobbyCode);
        if(other.tag == "Player"){
            //ChangeScene();
            //JoinLobby();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex+1);
            StoreNetworking.instance.JoinLobby(lobbyCode);
        }
    }

}
