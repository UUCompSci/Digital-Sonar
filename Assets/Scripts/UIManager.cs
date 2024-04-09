using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public GameObject torpedoButton;
    public GameObject surfaceButton;
    public GameObject silenceButton;
    public GameObject sonarButton;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getTorpedoButton() {
        return torpedoButton;
    }

    public GameObject getSurfaceButton() {
        return surfaceButton;
    }

    public GameObject getSilenceButton() {
        return silenceButton;
    }

    public GameObject getSonarButton() {
        return sonarButton;
    }
}
