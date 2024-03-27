using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class SubmarineLogicScript : MonoBehaviour
{
    public int movespeed = 5;
    public Transform movePoint;
    public int maxHealth;
    public int maxEnergy;
    private int energy;
    private int[] position;
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

    public int[] getPosition() {
        return position;
    }

    public void updatePosition(int[] move, bool report) {
        position = new int[2] {position[0] + move[0], position[1] + move[1]};
        Path.Node parent = path.getTails()[0];
        parent.addChild(new Path.Node(path.getTails()[0], move));
        movePoint.position += new Vector3(position[0] + move[0], position[1] + move[1], 0);
        if (report) {
            int[] lastMove = parent.getMove();
            path.updateDisplaySimple(lastMove, move, position, parent.getSilenceOut());
        }
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