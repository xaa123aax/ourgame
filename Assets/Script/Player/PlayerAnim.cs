using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class PlayerAnim : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject MiniMark;
    public Animator Player_ani;
    public bool isAction;
    public PlayerManager playerManager;
    public PlayerMove playerMove;
    public BoxCollider sword;
    public GameObject Sword, GunTeam, SwordTeam, PlayerTeam, Gun;
    public bool Attacking;
    public float Sp;
    public GameObject background, Victory, Defeat;
    public float wintime;
    public float playerweapon = 1;
    public int RedPlayNum;
    public int BluePlayNum;

    public GameObject gamemanager;
    PlayerUI playerUI;


    void Start()
    {
        background.SetActive(false);
        Victory.SetActive(false);
        Defeat.SetActive(false);
        Gun.SetActive(false);
        sword.enabled = false;
        Attacking = false;
        isAction = false;
        playerUI = GameObject.Find("PlayerUI" + photonView.Owner.NickName + photonView.ViewID).GetComponent<PlayerUI>();
        gamemanager = GameObject.Find("GameManager");
    }


    void Update()
    {


        if (photonView.IsMine)
        {
            if (!playerManager.IsDead)
            {
                if (Input.GetMouseButtonDown(0) && (Attacking == false || isAction == false))
                {
                    Sp = playerManager.PlayerSp;
                    Debug.Log(Sp);
                    if (Sp >= 40)
                    {
                        Attacking = true;
                        isAction = true;
                        Player_ani.SetBool("IsAttack", true);
                    }
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    playerManager.CoHp(40);
                }
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    Player_ani.SetBool("IsWalking", true);
                }
                else
                {
                    Player_ani.SetBool("IsWalking", false);
                }
            }
        }
    }
    #region Trigger判定
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (other.CompareTag("GameEnd"))
        {
            background.SetActive(true);
        }
        if (gameObject.tag == "RedPlayer")
        {
            if (other.CompareTag("BlueSword"))
            {
                playerManager.CoHp(40);
                if (playerManager.IsDead)
                {
                    gameObject.transform.position = GameObject.Find("RedTp").GetComponent<Transform>().position;
                }
            }
            if (other.CompareTag("BlueBall"))
            {
                playerManager.CoHp(30);
                if (playerManager.IsDead)
                {
                    gameObject.transform.position = GameObject.Find("RedTp").GetComponent<Transform>().position;
                }
            }
            if (other.CompareTag("RedWin"))
            {
                Victory.SetActive(true);
            }
            if (other.CompareTag("BlueWin"))
            {
                Defeat.SetActive(true);
            }
        }


        else if (gameObject.tag == "BluePlayer")
        {
            if (other.CompareTag("RedSword"))
            {
                playerManager.CoHp(40);
                if (playerManager.IsDead)
                {
                    gameObject.transform.position = GameObject.Find("BlueTp").GetComponent<Transform>().position;
                }
            }
            if (other.CompareTag("RedBall"))
            {
                playerManager.CoHp(30);
                if (playerManager.IsDead)
                {
                    gameObject.transform.position = GameObject.Find("BlueTp").GetComponent<Transform>().position;
                }
            }

            if (other.CompareTag("RedWin"))
            {
                Defeat.SetActive(true);
            }
            if (other.CompareTag("BlueWin"))
            {
                Victory.SetActive(true);
            }

        }
        if (other.CompareTag("RedTeam"))
        {
            playerUI.playerNameText.color = new Color(255, 0, 0);
            SwordTeam.tag = "RedSword";
       
            GunTeam.tag = "RedGun";
            gameObject.tag = "RedPlayer";
            PlayerTeam.tag = "RedPlayer";
            gameObject.transform.position = GameObject.Find("RedTp").GetComponent<Transform>().position;

        }
        if (other.CompareTag("BlueTeam"))
        {
            playerUI.playerNameText.color = new Color(0, 0, 255);
            BluePlayNum++;
            SwordTeam.tag = "BlueSword";
            GunTeam.tag = "BlueGun";
            gameObject.tag = "BluePlayer";
            PlayerTeam.tag = "BluePlayer";
            gameObject.transform.position = GameObject.Find("BlueTp").GetComponent<Transform>().position;
        }
        if (other.CompareTag("whiteteam") || Input.GetKeyDown(KeyCode.J))
        {

            playerUI.playerNameText.color = new Color(0, 0, 0);
            SwordTeam.tag = "Sword";
            gameObject.tag = "Player";
            PlayerTeam.tag = "Player";
            gameObject.transform.position = GameObject.Find("StartTp").GetComponent<Transform>().position;
        }
    }
    #endregion

    #region IPunObservable implementation  Photon傳值
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.tag);
            stream.SendNext(SwordTeam.tag);
            stream.SendNext(PlayerTeam.tag);
            stream.SendNext(playerUI.playerNameText.color.r);
            stream.SendNext(playerUI.playerNameText.color.g);
            stream.SendNext(playerUI.playerNameText.color.b);
        }
        else
        {
            this.tag = (string)stream.ReceiveNext();
            SwordTeam.tag = (string)stream.ReceiveNext();
            PlayerTeam.tag = (string)stream.ReceiveNext();
            playerUI.playerNameText.color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }
    #endregion
    public void AttackFalse()
    {

        Player_ani.SetBool("IsAttack", false);
    }


    public void AttackCoSp()
    {
        playerManager.CoSp(30);
    }



    public void AllEnd()
    {
        Attacking = false;
        Player_ani.SetBool("IsAttack", false);
        isAction = false;

    }
    public void SwordTrue()
    {
        sword.enabled = true;
    }
    public void SwordFalse()
    {
        sword.enabled = false;
    }
}


