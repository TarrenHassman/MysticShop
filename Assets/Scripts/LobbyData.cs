using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyData : MonoBehaviour
{
  public static LobbyData instance;
  public Lobby clientLobby;
  public Dictionary<string, DataObject> data = new();
  private void Awake()
  {
    instance = this;
    DontDestroyOnLoad(this);
  }
  public void changeLobby(Lobby lobby)
  {
    clientLobby = lobby;
  }
  public void AddData(string key, DataObject value)
  {
    if(data.ContainsKey(key))
    {
      data[key] = value;
    }
    else
    {
      data.Add(key, value);
    }
   
  }
}