using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class ExplodeEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Dis_Exp", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Dis_Exp()
    {
        Destroy(gameObject);
    }
}
