using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapCamera : MonoBehaviour
{
    public Transform mainCharacter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = mainCharacter.position + new Vector3(0,10,0);
    }
}
