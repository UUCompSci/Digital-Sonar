using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    int health;
    Color fullColor = Color.green;
    Color emptyColor = Color.red;

    public SpriteRenderer[] healthBarNotches;

    // Start is called before the first frame update
    void Start()
    {
        health = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayHealthGain(int healthGain) {
        for (int i = 0; i < healthGain; i++) {
            healthBarNotches[health].color = fullColor;
            health += 1;
        }
    }

    public void displayHealthLoss(int healthLoss) {
        for (int i = 1; i <= healthLoss; i++) {
            health -= 1;
            healthBarNotches[health].GetComponent<SpriteRenderer>().color = emptyColor;
        }
    }
}
