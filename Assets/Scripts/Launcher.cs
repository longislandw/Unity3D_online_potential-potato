/*  Put on sample scene -> canvas   */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;   //new import
using TMPro;
using Photon.Realtime;
using System.Linq;  //count() function

public class Launcher : MonoBehaviourPunCallbacks   //extend class from "MonoBehaviourPunCallbacks"
{

    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;

    [SerializeField] Transform roomListContent; //抓取物件
    [SerializeField] GameObject roomListItemPrefab;

    [SerializeField] Transform PlayerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;

    [SerializeField] GameObject startGameButton;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Debug.Log("Connecting To Master");
        PhotonNetwork.ConnectUsingSettings();   //連結到 Fixed region -> jp
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby();  //必須進入lobby才能建立房間 or 進入房間
        PhotonNetwork.AutomaticallySyncScene = true;    //要所有人都換場景
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }
    
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        //刪除先前的 playerlist
        foreach(Transform child in PlayerListContent)
        {
            Destroy(child.gameObject);
        }

        //進去遊戲後 複寫(override) 玩家名稱
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);    //只有host可以看到 -> StartGame Button
    }

    //if master離開房間了 -> photon將自動更換 host
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);    //只有host可以看到 -> StartGame Button
    }

    public /*override*/ void OnCreatedRoomFalied(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");   
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); //所有人都換一次場景，而不是只有host一直換
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) //給我們 room的資訊 -> Photon內建的class
    {
        /*
         * foreach 陳述式是用來逐一查看一個集合陣列用，
         * 以取得所需的資訊，但是不能用來加入或移除來源集合的項目， 
         * 如果想要加入或移除來源集合的項目須使用for 迴圈。
        */
        foreach (Transform trans in roomListContent) //clear the list everytime update
        {
            Destroy(trans.gameObject);  //每次更新，刪除原先的List
        }
        for(int i=0; i<roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            //Instantiate實例化(要生成的物件, 物件位置, 物件旋轉值);
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUP(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
