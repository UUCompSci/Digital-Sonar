using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject silenceButton;
    public GameObject sonarButton;
    public GameObject torpedoButton;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getSilenceButton() {
        return silenceButton;
    }

    public GameObject getSonarButton() {
        return sonarButton;
    }

    public GameObject getTorpedoButton() {
        return torpedoButton;
    }
}