using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreEntrance : MonoBehaviour
{
    public string lobbyCode;

    public int sceneBuildIndex = 0;

  protected void  OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex+1);
            StoreNetworking.instance.JoinLobby(lobbyCode);
        }
    }

}
enum Scenes
{
    menu = 0,
    customer = 1,
    merchant = 2,
}