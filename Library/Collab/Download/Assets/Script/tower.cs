using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower : MonoBehaviour
{

    public int TowerHp = 0;
    public float timeblue;
    public float timered;
    public float bluetower;
    public float redtower;

    #region Trigger判定
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("BlueSword"))
        {
            TowerHp++;
            TowerColor();
        }
        if (other.CompareTag("RedSword"))
        {
            TowerHp--;
            TowerColor();
        }

    }
    #endregion
    void Update()
    {
        if(gameObject.tag == "BlueTower")
        {
            timeblue = timeblue + Time.deltaTime;
        }
        if (gameObject.tag == "RedTower")
        {
            timered = timered + Time.deltaTime;
        }
    }
    public void TowerColor()
    {

        if (TowerHp >= 3)
        {
            TowerHp = 3;
            gameObject.tag = "BlueTower";
            GetComponentInChildren<MeshRenderer>().material.SetVector("_EmissionColor", Color.blue * 5f);

            bluetower = 1;
            redtower = 0;

        }
        if (TowerHp <= -3)
        {
            TowerHp = -3;
            gameObject.tag = "RedTower";
            GetComponentInChildren<MeshRenderer>().material.SetVector("_EmissionColor", Color.red * 5f);
            bluetower = 0;
            redtower = 1;
          
        }
        if (TowerHp == 0)
        {
            TowerHp = 0;
            gameObject.tag = "Tower";
            GetComponentInChildren<MeshRenderer>().material.SetVector("_EmissionColor", Color.white * 5f);
            bluetower = 0;
            redtower = 0;
        }
    }
}



