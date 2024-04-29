using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SilenceMoveTarget : MonoBehaviour
{
    public void OnClick() {
        GameObject submarine = gameObject.GetComponentInParent<SubmarineToken>().submarine;
        PlayerController player = submarine.GetComponent<PlayerController>();
        SubmarineLogicScript logicScript = submarine.GetComponentInChildren<SubmarineLogicScript>();
        GameLogicManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>();
        gameManager.spendAction(player);
        logicScript.useEnergy(gameManager.getSilenceEnergyCost());
        Vector2Int targetPosition = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
        player.Move(submarine.GetComponentInChildren<SubmarineLogicScript>(), targetPosition, "...");
        RadioOperator reportee = gameManager.getOpponentRadioOperator(player);
        if (gameManager.gainEnergyOnSilence()) {
            logicScript.gainEnergy(1);
        } 
        reportee.reportMove();
        player.clearCanvas();
    }
}
