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
        Vector2Int targetPosition = new Vector2Int((int)gameObject.transform.position[0], (int)gameObject.transform.position[1]);
        string move = submarine.GetComponent<PlayerController>().Move(submarine.GetComponentInChildren<SubmarineLogicScript>(), targetPosition, "...");
        RadioOperator reportee = GameObject.Find("Game Logic Manager").GetComponent<GameLogicManager>().getOpponentRadioOperator(submarine);
        reportee.reportMove(move);
        submarine.GetComponent<PlayerController>().clearCanvas();
    }
}
