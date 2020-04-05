using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadScript : MonoBehaviour
{
    public PlayerManager playerManager;
    public float deadtime=0;
    void Start()
    {

    }


    void Update()
    {
        if (gameObject.tag == "RedPlayer")
        {
            if (playerManager.IsDead)
            {
                gameObject.transform.position = GameObject.Find("RedTp").GetComponent<Transform>().position;
                deadtime = deadtime + Time.deltaTime;
            }


        }


        else if (gameObject.tag == "BluePlayer")
        {

            if (playerManager.IsDead)
            {
                gameObject.transform.position = GameObject.Find("BlueTp").GetComponent<Transform>().position;
                deadtime = deadtime + Time.deltaTime;
            }
        }


        if (deadtime >= 10)
        {
            playerManager.IsDead = false;
            playerManager.PlayerHp = 100;
            deadtime = 0;
        }
    }
}
