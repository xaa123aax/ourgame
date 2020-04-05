using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarkcolorChange : MonoBehaviour { 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

            if (GetComponentInParent<Transform>().tag == "RedPlayer")
                GetComponent<MeshRenderer>().material.color = Color.red;
            if (GetComponentInParent<Transform>().tag == "Player")
                GetComponent<MeshRenderer>().material.color = Color.white;
            if (GetComponentInParent<Transform>().tag == "BluePlayer")
                GetComponent<MeshRenderer>().material.color = Color.blue;
        
    }
}
