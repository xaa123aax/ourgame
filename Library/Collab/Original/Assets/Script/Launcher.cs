using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.libertier.PVPDEMO
{
    public class Launcher: MonoBehaviourPunCallbacks
    {
        [SerializeField] //公開非公共字段
        private byte maxPlayersPerRoom = 4;
        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private GameObject progressLabel;

        bool isConnecting;


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

            //如果我們不嘗試加入房間，我們什麼都不做。
            // isConnecting為false的情況通常是在您輸掉遊戲或退出遊戲時加載的，此級別加載時，將調用OnConnectedToMaster
            //我們什麼都不想做。
            if (isConnecting)
            {
                // #重要：我們首先要做的是加入一個潛在的現有房間。 如果有的話，否則，我們將使用OnJoinRandomFailed（）進行回調
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
               // PhotonNetwork.JoinRandomRoom();
            }
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

        public override void OnJoinedRoom()
        {
            Debug.Log("加入房間");

            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Room for 1");
            }

        }

        #endregion
    }
}
