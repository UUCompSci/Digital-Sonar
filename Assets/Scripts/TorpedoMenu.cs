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
        if (targetPosition.x < 0 || targetPosition.y < 0) {
            warningMessage.enabled = true;
            return;
        }
        gameLogicManager.createExplosion(targetPosition, parentSub, gameLogicManager.safeTorpedoes);
    }

    public void setTargetPosition_x(string x) {
        targetPosition.x = System.Convert.ToInt32(x);
    }

    public void setTargetPosition_y(string y) {
        targetPosition.y = System.Convert.ToInt32(y);
    }
}
