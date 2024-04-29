using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class ReportMoveTarget : MonoBehaviour
{
    public void OnClick() {
        GameObject submarine = gameObject.GetComponentInParent<SubmarineToken>().submarine;
        PlayerController player = submarine.GetComponent<PlayerController>();
        SubmarineLogicScript logicScript = submarine.GetComponentInChildren<SubmarineLogicScript>();
        GameLogicManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>();
        gameManager.spendAction(player);
        Vector2Int targetPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int move = player.Move(logicScript, targetPosition);
        RadioOperator reportee = gameManager.getOpponentRadioOperator(player);
        reportee.reportMove(move);
        logicScript.gainEnergy(1);
        player.clearCanvas();
    }
}
