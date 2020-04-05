using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerMarkColor : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {

            if (GetComponentInParent<Transform>().tag == "RedTower")
                GetComponent<MeshRenderer>().material.color = Color.red;
            if (GetComponentInParent<Transform>().tag == "Tower")
                GetComponent<MeshRenderer>().material.color = Color.white;
            if (GetComponentInParent<Transform>().tag == "BlueTower")
                GetComponent<MeshRenderer>().material.color = Color.blue;
 }
    
}
