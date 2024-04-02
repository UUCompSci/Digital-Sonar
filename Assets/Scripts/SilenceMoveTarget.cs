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
        Console.WriteLine(gameObject.name);
        Vector2Int targetPosition = new Vector2Int((int)gameObject.transform.position[0], (int)gameObject.transform.position[1]);
        Console.WriteLine("target position: " + targetPosition);
        string move = gameObject.transform.parent.parent.gameObject.GetComponent<PlayerController>().Move(gameObject.transform.parent.parent.gameObject.GetComponentInChildren<SubmarineLogicScript>(), targetPosition, "...");
        
        reportee.reportMove(move);
    }
}
