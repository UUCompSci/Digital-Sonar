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
    public GameObject[] UIList;
    public Tilemap[] tilemaps;
    public int mapWidth;
    public int mapHeight;
    public bool energyGainOnSilence;
    public int silenceEnergyCost;
    public int torpedoEnergyCost;
    public int sonarEnergyCost;
    public int roundCount;
    private int turnTracker;
    private enum gameState {
        WON,
        CONTINUE,
        PAUSED,
        QUIT
    };

    private gameState gameStateTracker = gameState.CONTINUE;



    // Start is called before the first frame update
    void Start()
    {
        turnScreens = GameObject.FindGameObjectsWithTag("TurnScreen");
        for (int i = 1; i < turnScreens.Length; i++) {
            turnScreens[i].SetActive(false);
        }
    }

    public PlayerController getOpponent(PlayerController queryingPlayer) {
        return turnList[(System.Array.IndexOf(turnList, queryingPlayer) + 1) % turnList.Length];
    }

    public RadioOperator getOpponentRadioOperator(GameObject queryingPlayer) {
        return radioOperators[(System.Array.IndexOf(turnList, queryingPlayer) + 1) % turnList.Length];
    }

    public void endCurrentTurn() {
        turnTracker = (turnTracker + 1) % turnList.Length;
        if (gameStateTracker != gameState.WON) {
            turnScreens[turnTracker].SetActive(true);
        };
    }

    public void endCurrentTurn(PlayerController opponent) {
        endCurrentTurn();
        opponent.gameObject.GetComponentInChildren<Canvas>().enabled = true;
        
    }

    public void startTurn(int i) {
        PlayerController player = turnList[i];
        SubmarineLogicScript logicScript = player.gameObject.GetComponentInChildren<SubmarineLogicScript>();
        UIManager buttonManager = UIList[i].GetComponent<UIManager>();
        UIList[i].SetActive(true);
        UIList[(i + 1) % turnList.Length].SetActive(false);
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
        tilemaps[(i + 1) % turnList.Length].GetComponent<TilemapRenderer>().enabled = false;
        player.gameObject.SetActive(true);
        turnList[(i + 1) % turnList.Length].gameObject.SetActive(false);
        radioOperators[i].GetComponentInChildren<Grid>().gameObject.GetComponentInChildren<TilemapRenderer>().enabled = true;
        radioOperators[(i + 1) % turnList.Length].GetComponentInChildren<Grid>().gameObject.GetComponentInChildren<TilemapRenderer>().enabled = false;
        player.displayMoveOptions();
        turnScreens[i].SetActive(false);
    }

    public bool gainEnergyOnSilence() {
        return energyGainOnSilence;
    }

    public int getSilenceEnergyCost() {
        return silenceEnergyCost;
    }
}
