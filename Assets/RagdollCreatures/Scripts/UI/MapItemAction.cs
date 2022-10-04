using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItemAction : MonoBehaviour
{
   
    public float rotSpeed = 30.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.forward * Time.deltaTime * rotSpeed);
    }

    
}
