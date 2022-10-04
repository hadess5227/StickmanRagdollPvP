using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnEnable()
    {
        Invoke("DestroyThis", 1f);
    }
    void Update()
    {
        
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
