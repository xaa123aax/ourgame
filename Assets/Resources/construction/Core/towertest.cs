using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class towertest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", Color.red * 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
