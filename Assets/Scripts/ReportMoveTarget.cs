using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class ReportMoveTarget : MonoBehaviour
{

    public RadioOperator reportee;
    public void OnClick() {
        int[] targetPosition = new int[2] {Convert.ToInt32(gameObject.transform.parent.position[0]), Convert.ToInt32(gameObject.transform.parent.position[1])};
        int[] move = gameObject.transform.parent.gameObject.GetComponent<PlayerController>().Move(gameObject.transform.parent.gameObject.GetComponentInChildren<SubmarineLogicScript>(), targetPosition);
        reportee.reportMove(move);
    }
}
