using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using QFSW.QC;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using System.Threading.Tasks;
using System;

public class StoreNetworking : MonoBehaviour
{

    public static StoreNetworking instance;
    private Lobby hostLobby;
    private Lobby clientLobby;
    private float heartbeatTimer;
    private float updateHeartbeatTimer;

    [SerializeField]
    private GameObject circle;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    protected void Update(){
        LobbyHeartbeatAsync();

    }


    private async void LobbyHeartbeatAsync(){
        if (hostLobby != null){
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f){
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
#nullable enable
    public async void CreateLobby(){
        //Get CID from PlayerPrefs
        string? cid = PlayerPrefs.HasKey("CID") ? PlayerPrefs.GetString("CID") : null;
        //Get Host name
        string hostName = PlayerPrefs.HasKey("HostName") ? PlayerPrefs.GetString("HostName") : Guid.NewGuid().ToString();
        //Get Host name
        string storeName = PlayerPrefs.HasKey("StoreName") ? PlayerPrefs.GetString("StoreName") : Guid.NewGuid().ToString();
        try
        {
        int maxPlayers = 4;
        string relay = await CreateRelay();
        LobbyData.instance.AddData("joinCode", new DataObject(DataObject.VisibilityOptions.Public,relay));
        LobbyData.instance.AddData("CID", new DataObject(DataObject.VisibilityOptions.Public,cid));
        CreateLobbyOptions options = new(){
            IsPrivate = false,
            Player = new Player() {
                Data = new Dictionary<string, PlayerDataObject>(){
                    {hostName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"Name")},

                }
            },
            Data = LobbyData.instance.data
        };
        hostLobby = await LobbyService.Instance.CreateLobbyAsync(storeName, maxPlayers, options);
        LobbyData.instance.changeLobby(hostLobby);

        }catch (LobbyServiceException e){
           Debug.Log(e);
    }}
#nullable disable
    public async Task<string> CreateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(15);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);          
            RelayServerData relayServerData = new (
                allocation,
                "dtls"
            );
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return joinCode;
            
        }catch (RelayServiceException e){
            return e.ToString();

       };
    }

    public async void UpdateLobby(string cid){
        hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id,
        new UpdateLobbyOptions{Data = new Dictionary<string, DataObject>(){
                {"CID", new DataObject(DataObject.VisibilityOptions.Public,cid)},
            }});
    }


    private async void UpdatePlayer(){
        await LobbyService.Instance.UpdatePlayerAsync(hostLobby.Id,AuthenticationService.Instance.PlayerId , new UpdatePlayerOptions{
            Data = new Dictionary<string, PlayerDataObject>(){
                {"Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"Name")},
            }
        });
    }

    [Command]
    private async void KickPlayer(int index){
        try{
            await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, hostLobby.Players[index].Id);
        }catch (LobbyServiceException e){
            Debug.Log(e);
    }}
    [Command]
    private async void DeleteLobby(){
        try{
            await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
        }catch (LobbyServiceException e){
                Debug.Log(e);
    }}


    //Client Functions


[Command]
    private async void ListLobbies(){
        try{
            //Make filter options variable and changeable by menu
            QueryLobbiesOptions options = new(){
                Count = 10,
                Filters = new List<QueryFilter>{
                    new QueryFilter(
                        QueryFilter.FieldOptions.AvailableSlots,
                        "0",
                        QueryFilter.OpOptions.GT
                    )
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(
                        false,
                        QueryOrder.FieldOptions.Created
                    )
                }
            };
            QueryResponse query = await LobbyService.Instance.QueryLobbiesAsync(options);
            int index = 0;
            foreach (Lobby lobby in query.Results){
                GameObject obj = Instantiate(circle, new Vector3(index++,1,2), Quaternion.identity);
                StoreEntrance trigger = obj.AddComponent<StoreEntrance>();
                trigger.lobbyCode = lobby.Id;
                Debug.Log(lobby.Id);
            }
        }catch (LobbyServiceException e){
            Debug.Log(e);
    }}
    


    private async void HandleLobbyPollForUpdate(){
        if (hostLobby != null){
            updateHeartbeatTimer -= Time.deltaTime;
            if (updateHeartbeatTimer < 0f){
                float updateHeartbeatTimerMax = 1.1f;
                updateHeartbeatTimer = updateHeartbeatTimerMax;
                clientLobby = await LobbyService.Instance.GetLobbyAsync(clientLobby.Id);
            }
        }
    }


    public async void JoinLobby(string lobbyCode){
        clientLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyCode);
        LobbyData.instance.clientLobby = clientLobby;
        JoinRelay(clientLobby.Data["joinCode"].Value);
        
    }

    public async void JoinRelay(string joinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                        RelayServerData relayServerData = new (
                joinAllocation,
                "dtls"
            );
             NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }catch (RelayServiceException e){
            Debug.Log(e);
    }}

    public async void LeaveLobby(){
         try{
        await LobbyService.Instance.RemovePlayerAsync(clientLobby.Id, AuthenticationService.Instance.PlayerId);
        }catch (LobbyServiceException e){
            Debug.Log(e);
    }}


}

//TODO: Organize between client and host functions