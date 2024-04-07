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
        if (Math.Abs(gameObject.transform.position.x - enemySubmarine.gameObject.transform.parent.position.x) <= directHitRange 
        && Math.Abs(gameObject.transform.position.y - enemySubmarine.gameObject.transform.parent.position.y) <= directHitRange) {
            Console.WriteLine($"Direct hit! {directHitDamage} damage dealt!");
            return directHitDamage;
        } else if (Math.Abs(gameObject.transform.position.x - enemySubmarine.gameObject.transform.parent.position.x) <= indirectHitRange 
        && Math.Abs(gameObject.transform.position.y - enemySubmarine.gameObject.transform.parent.position.y) <= indirectHitRange) {
            Console.WriteLine($"Indirect hit! {indirectHitDamage} damage dealt!");
            return indirectHitDamage;
        } else {
            Console.WriteLine("Miss! No damage dealt");
            return 0;
        };
    }

}
