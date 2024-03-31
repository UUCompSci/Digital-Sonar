using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class SubmarineLogicScript : MonoBehaviour
{
    public float moveSpeed = .5f;
    public Transform movePoint;
    public int maxHealth;
    public int maxEnergy;
    private int energy;
    private int health;
    private Path path;

    void Start() {
        movePoint.parent = null;
    }

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

    public int getHealth() {
        return health;
    }

    public Path getPath() {
        return path;
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