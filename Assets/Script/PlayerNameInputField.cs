using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

namespace Com.libertier.PVPDEMO
{
    /// <summary>
    /// 玩家名稱輸入字段。 讓用戶輸入他的名字，將出現在遊戲中玩家上方。
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        // 存儲PlayerPref鍵以避免輸入錯誤
        //const 可以避免項目被改變
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {

            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///設置玩家的名稱，並將其保存在PlayerPrefs中以備將來使用(以後會優先顯示最後輸入過的名字)
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            // #重要
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
        
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}