using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.libertier.PVPDEMO
{
    public class Launcher: MonoBehaviourPunCallbacks
    {
        [SerializeField] //公開非公共字段
        private byte maxPlayersPerRoom = 20;
        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private GameObject progressLabel;

        bool isConnecting;
        public InputField CreateRoomTF;
        public InputField JoinRoomTF;
        public GameObject connectedScreen;

        #region 
        #endregion

        #region 


        string gameVersion = "1";
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {

            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods


        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // 我們檢查是否連接，是否加入，否則啟動與服務器的連接。
            if (PhotonNetwork.IsConnected)
            {
                //＃至關重要，我們現在需要嘗試加入隨機房間。 如果失敗，我們將在OnJoinRandomFailed（）中得到通知，並創建一個。
                PhotonNetwork.JoinRandomRoom();
            }
            else
                // #重要，我們必須首先連接到Photon Online Server。
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();

            //確認是否連結
            isConnecting = true;
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks


        public override void OnConnectedToMaster()
        {

            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }

        public override void OnJoinedLobby()
        {
            connectedScreen.SetActive(true);
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("連結失敗", cause);
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {   
            Debug.Log("創建房間");

            // ＃嚴重：我們無法加入隨機房間，可能不存在或房間已滿。 不用擔心，我們創建了一個新房間。
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        public void OnClick_ConnectBtn()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public void OnClick_JoinRoom()
        {
            PhotonNetwork.JoinRoom(JoinRoomTF.text, null);
        }

        public void OnClick_CreateRoom()
        {
            PhotonNetwork.CreateRoom(CreateRoomTF.text, new RoomOptions { MaxPlayers = 20 }, null);
        }


        public override void OnJoinedRoom()
        {
            Debug.Log("加入房間");
            PhotonNetwork.LoadLevel(1);

        }

        #endregion
    }
}
