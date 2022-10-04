using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTransform;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(followTransform)
            this.transform.position = new Vector3(followTransform.position.x, followTransform.position.y, this.transform.position.z);
    }

    private void Update()
    {

    }
}
