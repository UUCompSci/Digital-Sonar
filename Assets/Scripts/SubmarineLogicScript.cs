using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class SubmarineLogicScript : MonoBehaviour
{
    public float moveSpeed = .5f;
    public Transform movePoint;
    public int maxHealth;
    public int maxEnergy;
    private int energy;
    private int health;
    private Path path = null;
    public EnergyGauge energyGauge;

    [SerializeField]
    private Tilemap pathTilemap;

    void Start() {
        movePoint.parent = null;
    }

    public int useEnergy(int n) {
        if (n > energy) {
            return -1;
        } else {
            energy -= n;
            energyGauge.displayEnergyLoss(n);
            return energy;
        };
    }

    public void gainEnergy(int n) {
        if (n > maxEnergy - energy) {
            n = maxEnergy - energy;
        }
        if (n > 0) {
            energy += n;
            energyGauge.displayEnergyGain(n);
        } else {
            Debug.Log("gainEnergy only accepts positive integer values, try again.");
        };
    }

    public int getEnergy() {
        return energy;
    }

    public int getHealth() {
        return health;
    }

    public void setPath(Vector2Int move) {
        path = new Path(move, new Vector2Int((int)transform.parent.position.x, (int)transform.parent.position.y), pathTilemap);
    }
    public Path getPath() {
        return path;
    }

    public int dealDamage(int damage) {
        if (damage > 0 && damage <= health) {
            health -= damage;
            return health;
        } else if (damage > 0) {
            health = 0;
            return health;
        } else {
            Debug.Log("dealDamage only accepts positive integer values, try again.");
        }
        return health;
    }
}