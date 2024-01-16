using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LogicScript : MonoBehaviour
{
    private int energy;
    public int maxEnergy;
    private int[] position;
    public int maxHealth;
    private int health;

    public int useEnergy(int n) {
        if (n > energy) {
            return -1;
        } else {
            energy -= n;
            return energy;
        };
    }

    public void gainEnergy(int n) {
        if (n > 0) {
            energy += n;
            if (energy > maxEnergy) {
                energy = maxEnergy;
            };
        } else {
            Console.WriteLine("gainEnergy only accepts positive integer values, try again.");
        };
    }

    public int[] getPosition() {
        return position;
    }

    public void updatePosition(int[] newPosition) {
        position = newPosition;
    }

    public int getHealth() {
        return health;
    }

    public int dealDamage(int damage) {
        if (damage > 0) {
            health -= damage;
            return health;
        };
        Console.WriteLine("dealDamage only accepts positive integer values, try again.");
        return maxHealth + 1;
    }

}
