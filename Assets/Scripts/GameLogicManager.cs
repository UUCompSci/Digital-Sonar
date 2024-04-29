using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class GameLogicManager : MonoBehaviour
{
    public PlayerController[] turnList;
    public RadioOperator[] radioOperators;
    public GameObject[] turnScreens;
    public GameObject[] winScreens;
    public UIManager[] UIList;
    public Tilemap[] tilemaps;
    public int mapWidth;
    public int mapHeight;
    public bool energyGainOnSilence;
    public int silenceEnergyCost;
    public int torpedoEnergyCost;
    public int sonarEnergyCost;
    public int roundCount;
    private int turnTracker;
    public int directHitDamage;
    public int indirectHitDamage;
    public int directHitRange;
    public int indirectHitRange;
    public bool safeTorpedoes;
    public bool safeMines;



    // Start is called before the first frame update
    void Start()
    {
        turnScreens = GameObject.FindGameObjectsWithTag("TurnScreen");
        for (int i = 1; i < turnScreens.Length; i++) {
            turnScreens[i].SetActive(false);
        }
        winScreens = GameObject.FindGameObjectsWithTag("WinScreen");
        foreach (GameObject winScreen in winScreens) {
            winScreen.SetActive(false);
        }
    }

    public PlayerController getOpponent(PlayerController queryingPlayer) {
        return turnList[(System.Array.IndexOf(turnList, queryingPlayer) + 1) % turnList.Length];
    }

    public RadioOperator getOpponentRadioOperator(PlayerController queryingPlayer) {
        return radioOperators[(System.Array.IndexOf(turnList, queryingPlayer) + 1) % turnList.Length];
    }

    public UIManager getUIManager(PlayerController player) {
        return UIList[System.Array.IndexOf(turnList, player)];
    }

    public void endCurrentTurn() {
        turnList[turnTracker].clearCanvas();
        turnTracker = (turnTracker + 1) % turnList.Length;
        turnScreens[turnTracker].SetActive(true);
    }

    public void spendAction(PlayerController queryingPlayer) {
        UIManager buttonManager = UIList[System.Array.IndexOf(turnList, queryingPlayer)];
        buttonManager.getTorpedoButton().GetComponent<Button>().interactable = false;
        buttonManager.getSurfaceButton().GetComponent<Button>().interactable = false;
        buttonManager.getSilenceButton().GetComponent<Button>().interactable = false;
        buttonManager.getSonarButton().GetComponent<Button>().interactable = false;
    }

    public void startTurn(int i) {
        PlayerController player = turnList[i];
        SubmarineLogicScript logicScript = player.gameObject.GetComponentInChildren<SubmarineLogicScript>();
        UIManager buttonManager = UIList[i];
        buttonManager.getSurfaceButton().GetComponent<Button>().interactable = true;
        buttonManager.gameObject.GetComponent<Canvas>().enabled = true;
        buttonManager.GetComponentInChildren<ViewSwapper>().radioOperatorPath.enabled = true;
        foreach (SpriteRenderer slot in buttonManager.gameObject.GetComponentInChildren<EnergyGauge>().energyGaugeSlots) {
            slot.enabled = true;
        }
        foreach (SpriteRenderer notch in buttonManager.gameObject.GetComponentInChildren<HealthBar>().healthBarNotches) {
            notch.enabled = true;
        }
        GameObject lastTurnButtonsManagerObject = UIList[(i + 1) % turnList.Length].gameObject;
        lastTurnButtonsManagerObject.GetComponent<Canvas>().enabled = false;
        foreach (SpriteRenderer slot in lastTurnButtonsManagerObject.GetComponentInChildren<EnergyGauge>().energyGaugeSlots) {
            slot.enabled = false;
        }
        foreach (SpriteRenderer notch in lastTurnButtonsManagerObject.GetComponentInChildren<HealthBar>().healthBarNotches) {
            notch.enabled = false;
        }
        if (logicScript.getEnergy() >= silenceEnergyCost) {
            buttonManager.getSilenceButton().GetComponent<Button>().interactable = true;
        } else {
            buttonManager.getSilenceButton().GetComponent<Button>().interactable = false;
        }
        if (logicScript.getEnergy() >= torpedoEnergyCost) {
            buttonManager.getTorpedoButton().GetComponent<Button>().interactable = true;
        } else {
            buttonManager.getTorpedoButton().GetComponent<Button>().interactable = false;
        }
        if (logicScript.getEnergy() >= sonarEnergyCost) {
            buttonManager.getSonarButton().GetComponent<Button>().interactable = true;
        } else {
            buttonManager.getSonarButton().GetComponent<Button>().interactable = false;
        }
        tilemaps[i].GetComponent<TilemapRenderer>().enabled = true;
        tilemaps[(i + 1) % tilemaps.Length].GetComponent<TilemapRenderer>().enabled = false;
        player.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        turnList[(i + 1) % turnList.Length].gameObject.GetComponent<SpriteRenderer>().enabled = false;
        UIList[(i + 1) % turnList.Length].gameObject.GetComponentInChildren<ViewSwapper>().radioOperatorPath.enabled = false;
        UIList[(i + 1) % turnList.Length].gameObject.GetComponentInChildren<ViewSwapper>().radioOperatorStartsGrid.enabled = false;
        player.displayMoveOptions();
        turnScreens[i].SetActive(false);
    }

    public bool gainEnergyOnSilence() {
        return energyGainOnSilence;
    }

    public int getSilenceEnergyCost() {
        return silenceEnergyCost;
    }

    public void createExplosion(Vector3Int targetPosition, PlayerController safeSub, bool safeExplosion) {
        // trigger animation
        foreach (PlayerController sub in turnList) {
            if (sub != safeSub || !safeExplosion) {
                SubmarineLogicScript logicScript = sub.gameObject.GetComponentInChildren<SubmarineLogicScript>();
                if (System.Math.Abs(targetPosition.x - sub.gameObject.transform.position.x) <= directHitRange 
                && System.Math.Abs(targetPosition.y - sub.gameObject.transform.position.y) <= directHitRange) {
                    Debug.Log($"Direct hit! {directHitDamage} damage dealt!");
                    logicScript.dealDamage(directHitDamage);
                } else if (System.Math.Abs(targetPosition.x - sub.gameObject.transform.position.x) <= indirectHitRange 
                && System.Math.Abs(targetPosition.y - sub.gameObject.transform.position.y) <= indirectHitRange) {
                    Debug.Log($"Indirect hit! {indirectHitDamage} damage dealt!");
                    logicScript.dealDamage(indirectHitDamage);
                } else {
                    Debug.Log("Miss! No damage dealt");
                };
            }
        }
    }

    public void declareLoss(PlayerController player) {
        winScreens[(System.Array.IndexOf(turnList, player) + 1) % turnList.Length].SetActive(true);
    }
}
