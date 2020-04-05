using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Photon.Pun;



public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{


    #region IPunObservable implementation
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

            stream.SendNext(PlayerHp);
            stream.SendNext(playhp.localScale.x);
            stream.SendNext(playhp.localScale.y);
            stream.SendNext(playhp.localScale.z);
        }
        else
        {
            this.PlayerHp = (float)stream.ReceiveNext();
            playhp.localScale = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }

    #endregion

    #region Private Fields
    public HpUI hpui;
    public SpUI spui;
    public float PlayerHp = 100;
    public float PlayerSp = 100;
    public bool IsDead = false;
    public PlayerAnim playerAnim;
    public GameObject PlayerMainCamera;
    public GameObject MiniMark;
    public GameObject MiniMarkMyself;
    public GameObject MiniMapCamera;
    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    public GameObject PlayerUiPrefab;
    public RectTransform playhp;
    public GameObject PlayerUI;
    public Score score;

    #endregion
    public void Start()
    {
        PlayerUI.SetActive(false);
        PlayerMainCamera.SetActive(false);
        MiniMapCamera.SetActive(false);
        MiniMarkMyself.SetActive(false);

        if (photonView.IsMine)
        {
            PlayerMainCamera.SetActive(true);
            PlayerUI.SetActive(true);
            MiniMapCamera.SetActive(true);
            MiniMarkMyself.SetActive(true);
            MiniMark.SetActive(false);
        }

        if (PlayerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            playhp = GameObject.Find("PlayerUI" + photonView.Owner.NickName + photonView.ViewID).GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

    }
    public void Update()
    {

        if (PlayerHp <= 0)
        {
            PlayerHp = 0;
            IsDead = true;
        }

        

        
    }

    public void CoHp(float damage)
    {
        PlayerHp -= damage;
        hpui.cohp(damage);

    }
    public void CoSp(float damage)
    {
        PlayerSp -= damage;
        spui.CoSp(damage);
        if (PlayerSp < 0)
        {
            PlayerSp = 0;
        }
    }



}

