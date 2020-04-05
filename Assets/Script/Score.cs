using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviourPunCallbacks
{
    public float BlueScore;
    public float RedScore;
    public float BlueTower;
    public float RedTower;

    public Text BlueNumber;
    public Text RedNumber;
    public Text BlueTowerNumber;
    public Text RedTowerNumber;

    public tower tower1;
    public tower tower2;
    public tower tower3;
    public tower tower4;
    public tower tower5;
    public GameObject redwin;
    public GameObject bluewin;
    public GameObject GameEnd;
    public float endtime;


    void Start()
    {
        redwin.SetActive(false);
        bluewin.SetActive(false);
        GameEnd.SetActive(false);
    }
    void Update()
    {
        BlueScore = tower1.timeblue + tower2.timeblue + tower3.timeblue + tower4.timeblue+ tower5.timeblue;
        RedScore = tower1.timered + tower2.timered + tower3.timered + tower4.timered+ tower5.timered;
        BlueTower = tower1.bluetower + tower2.bluetower + tower3.bluetower + tower4.bluetower+ tower5.bluetower;
        RedTower = tower1.redtower + tower2.redtower + tower3.redtower + tower4.redtower+ tower5.redtower;

        if (BlueScore >= 100)
        {
            BlueScore = 100;
            GameEnd.SetActive(true);
            bluewin.SetActive(true);
            endtime = endtime + Time.deltaTime;
            if (endtime >= 2)
            {

                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(2);
            }

        }


        if (RedScore >= 100)
        {
            RedScore = 100;
            GameEnd.SetActive(true);
            redwin.SetActive(true);
            endtime = endtime + Time.deltaTime;
            if (endtime >= 2)
            {

                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(2);
            }
        }



        BlueNumber.text = BlueScore.ToString("000");
        RedNumber.text = RedScore.ToString("000");
        BlueTowerNumber.text = BlueTower.ToString("0");
        RedTowerNumber.text = RedTower.ToString("0");



    }


}

