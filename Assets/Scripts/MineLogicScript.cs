using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MineLogicScript : MonoBehaviour
{
    public int[] position;
    public int directHitDamage;
    public int indirectHitDamage;
    public int directHitRange;
    public int indirectHitRange;
    
    public int detonate(SubmarineLogicScript enemySubmarine) {
        if (Math.Abs(position[0] - enemySubmarine.getPosition()[0]) <= directHitRange && Math.Abs(position[1] - enemySubmarine.getPosition()[1]) <= directHitRange) {
            Console.WriteLine($"Direct hit! {directHitDamage} damage dealt!");
            return directHitDamage;
        } else if (Math.Abs(position[0] - enemySubmarine.getPosition()[0]) <= indirectHitRange && Math.Abs(position[1] - enemySubmarine.getPosition()[1]) <= indirectHitRange) {
            Console.WriteLine($"Indirect hit! {indirectHitDamage} damage dealt!");
            return indirectHitDamage;
        } else {
            Console.WriteLine("Miss! No damage dealt");
            return 0;
        };
    }

}
