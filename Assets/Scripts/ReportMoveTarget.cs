using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class ReportMoveTarget : MonoBehaviour
{
    Transform parentTransform;
    public GameObject submarine;
    public RadioOperator reportee;
    public void OnClick() {
        int[] move = submarine.GetComponent<PlayerController>().Move(submarine.GetComponentInChildren<SubmarineLogicScript>(), new int[] {Convert.ToInt32(parentTransform.position[0]), Convert.ToInt32(parentTransform.position[1])});
        reportee.reportMove(move);
    }
}
