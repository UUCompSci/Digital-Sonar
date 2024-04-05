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
        GameLogicManager gameLogicManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>();
        GameObject submarine = gameObject.GetComponentInParent<SubmarineToken>().submarine;
        SubmarineLogicScript logicScript = submarine.GetComponentInChildren<SubmarineLogicScript>();
        logicScript.useEnergy(gameLogicManager.getSilenceEnergyCost());
        Vector2Int targetPosition = new Vector2Int((int)gameObject.transform.position[0], (int)gameObject.transform.position[1]);
        submarine.GetComponent<PlayerController>().Move(submarine.GetComponentInChildren<SubmarineLogicScript>(), targetPosition, "...");
        RadioOperator reportee = gameLogicManager.getOpponentRadioOperator(submarine);
        if (gameLogicManager.gainEnergyOnSilence()) {
            logicScript.gainEnergy(1);
        } 
        reportee.reportMove();
        submarine.GetComponent<PlayerController>().clearCanvas();
    }
}
