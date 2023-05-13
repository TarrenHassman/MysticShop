using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

     public void StartHost(){
        StoreNetworking.instance.CreateLobby();
            UnityEngine.SceneManagement.SceneManager.LoadScene((int)Scenes.merchant);
            }

       public void StartCustomer(){
            UnityEngine.SceneManagement.SceneManager.LoadScene((int)Scenes.customer);
       }
}

