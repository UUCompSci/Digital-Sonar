using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SilenceMoveTarget : MonoBehaviour
{
    Transform parentTransform;
    public GameObject submarine;
    public RadioOperator reportee;
    public void OnClick() {
        int[] targetPosition = {Convert.ToInt32(parentTransform.position[0]), Convert.ToInt32(parentTransform.position[1])};
        string move = submarine.GetComponent<PlayerController>().Move(submarine.GetComponentInChildren<SubmarineLogicScript>(), targetPosition, "...");
        reportee.reportMove(move);
    }
}
