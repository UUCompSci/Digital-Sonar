using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineToken : MonoBehaviour
{
    public GameObject submarine;
    
    // Start is called before the first frame update
    void Start()
    {
        submarine = transform.parent.gameObject;
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
