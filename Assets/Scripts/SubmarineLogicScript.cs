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
    [SerializeField] private int energy;
    [SerializeField] private int health;
    private Path path;
    public EnergyGauge energyGauge;
    public HealthBar healthBar;
    [SerializeField] private Tilemap pathTilemap;

    void Start() {
        movePoint.SetParent(null);
        path = gameObject.transform.parent.GetComponentInChildren<Path>();
        health = maxHealth;
        energy = 0;
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
        }
    }

    public int getEnergy() {
        return energy;
    }

    public int getHealth() {
        return health;
    }

    public void setPath(Vector2Int move) {
        Path newPath = path.gameObject.AddComponent<Path>();
        newPath.pathTilemap = path.pathTilemap;
        newPath.elimTilemap = path.elimTilemap;
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
        newPath.pathTilemap = path.pathTilemap;
        newPath.elimTilemap = path.elimTilemap;
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
        newPath.elimTile = path.elimTile;
        newPath.startPath(move);
        Destroy(path);
        path = newPath;
    }

    public ref Path getPath() {
        return ref path;
    }

    public void clearPath() {
        path.clearPath();
    }

    public int dealDamage(int damage) {
        Debug.Log("dealing " + damage + " damage");
        if (damage >= health) {
            health = 0;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>().declareLoss(transform.parent.GetComponent<PlayerController>());
        } else if (damage > 0) {
            health -= damage;
        }
        healthBar.displayHealthLoss(damage);
        return health;
    }
}