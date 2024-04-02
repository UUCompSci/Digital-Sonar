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
        Vector2Int targetPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int move = submarine.GetComponent<PlayerController>().Move(submarine.GetComponentInChildren<SubmarineLogicScript>(), targetPosition);
        RadioOperator reportee = GameObject.Find("Game Logic Manager").GetComponent<GameLogicManager>().getOpponentRadioOperator(submarine);
        reportee.reportMove(move);
        submarine.GetComponent<PlayerController>().clearCanvas();
    }
}
