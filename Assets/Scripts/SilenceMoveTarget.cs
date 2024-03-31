using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SilenceMoveTarget : MonoBehaviour
{
    public RadioOperator reportee;
    public void OnClick() {
        int[] targetPosition = {Convert.ToInt32(gameObject.transform.parent.position[0]), Convert.ToInt32(gameObject.transform.parent.position[1])};
        string move = gameObject.transform.parent.gameObject.GetComponent<PlayerController>().Move(gameObject.transform.parent.gameObject.GetComponentInChildren<SubmarineLogicScript>(), targetPosition, "...");
        reportee.reportMove(move);
    }
}
