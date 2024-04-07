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
    private Path path;
    public EnergyGauge energyGauge;

    [SerializeField]
    private Tilemap pathTilemap;

    void Start() {
        movePoint.SetParent(null);
        path = gameObject.transform.parent.GetComponentInChildren<Path>();
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
        Path newPath = path.gameObject.AddComponent<Path>();
        newPath.tilemap = path.tilemap;
        newPath.straight = path.straight;
        newPath.corner = path.corner;
        newPath.endpoint = path.endpoint;
        newPath.start = path.start;
        newPath.threeWay = path.threeWay;
        newPath.branching = path.branching;
        newPath.forking = path.forking;
        newPath.straightSilenceIn = path.straightSilenceIn;
        newPath.straightSilenceOut = path.straightSilenceOut;
        newPath.cornerSilenceIn = path.cornerSilenceIn;
        newPath.cornerSilenceOut = path.cornerSilenceOut;
        newPath.silenceEndpoint = path.silenceEndpoint;
        newPath.startPath(move);
        Destroy(path);
        path = newPath;
    }

    public void setPath(Vector2Int move, Vector2Int startingPosition) {
        Path newPath = path.gameObject.AddComponent<Path>();
        newPath.setStartingPosition(startingPosition);
        newPath.tilemap = path.tilemap;
        newPath.straight = path.straight;
        newPath.corner = path.corner;
        newPath.endpoint = path.endpoint;
        newPath.start = path.start;
        newPath.threeWay = path.threeWay;
        newPath.branching = path.branching;
        newPath.forking = path.forking;
        newPath.straightSilenceIn = path.straightSilenceIn;
        newPath.straightSilenceOut = path.straightSilenceOut;
        newPath.cornerSilenceIn = path.cornerSilenceIn;
        newPath.cornerSilenceOut = path.cornerSilenceOut;
        newPath.silenceEndpoint = path.silenceEndpoint;
        newPath.startPath(move);
        Destroy(path);
        path = newPath;
    }

    public Path getPath() {
        return path;
    }

    public void clearPath() {
        path.clearPath();
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