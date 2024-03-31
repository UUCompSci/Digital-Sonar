using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MineLogicScript : MonoBehaviour
{
    public int directHitDamage;
    public int indirectHitDamage;
    public int directHitRange;
    public int indirectHitRange;
    
    public int detonate(SubmarineLogicScript enemySubmarine) {
        if (Math.Abs(gameObject.transform.position[0] - enemySubmarine.gameObject.transform.parent.position[0]) <= directHitRange && Math.Abs(gameObject.transform.position[1] - enemySubmarine.gameObject.transform.parent.position[1]) <= directHitRange) {
            Console.WriteLine($"Direct hit! {directHitDamage} damage dealt!");
            return directHitDamage;
        } else if (Math.Abs(gameObject.transform.position[0] - enemySubmarine.gameObject.transform.parent.position[0]) <= indirectHitRange && Math.Abs(gameObject.transform.position[1] - enemySubmarine.gameObject.transform.parent.position[1]) <= indirectHitRange) {
            Console.WriteLine($"Indirect hit! {indirectHitDamage} damage dealt!");
            return indirectHitDamage;
        } else {
            Console.WriteLine("Miss! No damage dealt");
            return 0;
        };
    }

}
