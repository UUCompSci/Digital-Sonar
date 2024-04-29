using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int health;
    Color fullColor = Color.green;
    Color emptyColor = Color.red;

    public SpriteRenderer[] healthBarNotches;

    // Start is called before the first frame update
    void Start()
    {
        health = healthBarNotches.Length;
        for (int i = 0; i < healthBarNotches.Length; i++) {
            healthBarNotches[i].color = fullColor;
        }
    }

    public void displayHealthLoss(int healthLoss) {
        Debug.Log("Health Loss: " + healthLoss);
        for (int i = 1; i <= healthLoss; i++) {
            health -= 1;
            Debug.Log("Health Bar Notch: " + health);
            healthBarNotches[health].color = emptyColor;
        }
    }
}
