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
        for (int i = 0; i < healthBarNotches.Length; i++) {
            healthBarNotches[i].color = fullColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void displayHealthLoss(int healthLoss) {
        for (int i = 1; i <= healthLoss; i++) {
            health -= 1;
            healthBarNotches[health].color = emptyColor;
        }
    }
}
