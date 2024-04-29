using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TorpedoMenu : MonoBehaviour
{
    [SerializeField] private Vector3Int targetPosition = new Vector3Int(-1, -1, 0);
    public TMP_Text warningMessage;
    private GameLogicManager gameLogicManager;
    public PlayerController parentSub;

    public void Start() {
        gameLogicManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>();
    }

    public void fireTorpedo() {
        parentSub.GetComponentInChildren<SubmarineLogicScript>().useEnergy(gameLogicManager.torpedoEnergyCost);
        gameLogicManager.spendAction(parentSub);
        if (targetPosition.x < 0 || targetPosition.y < 0) {
            warningMessage.enabled = true;
            return;
        }
        parentSub.clearCanvas();
        reportee = gameLogicManager.getOpponentRadioOperator(getOpponent(parentSub));
        reportee.reportTorpedo(new Vector2Int(targetPosition.x, targetPosition.y), gameLogicManager.createExplosion(new Vector3Int(targetPosition.x - 5, targetPosition.y - 6), parentSub, gameLogicManager.safeTorpedoes));
    }

    public void setTargetPosition_x() {
        targetPosition.x = System.Convert.ToInt32(gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text);
    }

    public void setTargetPosition_y() {
        targetPosition.y = System.Convert.ToInt32(gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_InputField>().text);
    }
}
