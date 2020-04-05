using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationControll : MonoBehaviour
{
    Quaternion TheRotation;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.transform.eulerAngles = new Vector3(90,0, 180 - Camera.main.transform.eulerAngles.y);
        }
        else
        {
            this.transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }
}
