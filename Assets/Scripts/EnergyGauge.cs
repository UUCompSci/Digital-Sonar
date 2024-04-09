using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyGauge : MonoBehaviour
{
    int energy;
    Color fullColor = Color.green;
    Color emptyColor = Color.red;

    public SpriteRenderer[] energyGaugeSlots;

    // Start is called before the first frame update
    void Start()
    {
        energy = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayEnergyGain(int energyGain) {
        for (int i = 0; i < energyGain; i++) {
            energyGaugeSlots[energy].color = fullColor;
            energy += 1;
        }
    }

    public void displayEnergyLoss(int energyLoss) {
        for (int i = 1; i <= energyLoss; i++) {
            energy -= 1;
            energyGaugeSlots[energy].color = emptyColor;
        }
    }
}
