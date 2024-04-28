using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MineLogicScript : MonoBehaviour
{
    GameLogicManager gameLogicManager;
    PlayerController parentSub;
    
    public void Start() {
        gameLogicManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>();
    }

    public void detonate() {
        gameLogicManager.createExplosion(new Vector3Int((int) transform.position.x, (int) transform.position.y), parentSub, gameLogicManager.safeMines);
        Destroy(gameObject);
    }

}
