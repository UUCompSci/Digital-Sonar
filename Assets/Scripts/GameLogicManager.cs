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
    public GameObject player1Submarine;
    public GameObject player2Submarine;
    public RadioOperator player1RadioOperator;
    public RadioOperator player2RadioOperator;
    public GameObject player1UI;
    public GameObject player2UI;
    public int mapWidth;
    public int mapHeight;
    public bool energyGainOnSilence;
    public int silenceEnergyCost;
    public int torpedoEnergyCost;
    public int sonarEnergyCost;
    public int roundCount;
    private int turnTracker;
    public GameObject[] turnScreens;
    private PlayerController[] turnList;
    private GameObject[] UIList;
    public Tilemap[] tilemaps;
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
        turnList = new PlayerController[] {player1Submarine.GetComponent<PlayerController>(), player2Submarine.GetComponent<PlayerController>()};
        UIList = new GameObject[] {player1UI, player2UI};
        turnScreens = GameObject.FindGameObjectsWithTag("TurnScreen");
        for (int i = 0; i < turnScreens.Length; i++) {
            turnScreens[i].SetActive(false);
        }
    }

    public GameObject getOpponent(GameObject queryingPlayer) {
        if (queryingPlayer == player1Submarine) {
            return player2Submarine;
        } else {
            return player1Submarine;
        }
    }

    public RadioOperator getOpponentRadioOperator(GameObject queryingPlayer) {
        if (queryingPlayer == player1Submarine) {
            return player2RadioOperator;
        } else {
            return player1RadioOperator;
        }
    }

    public void endCurrentTurn() {
        turnTracker = (turnTracker + 1) % turnList.Length;
        if (gameStateTracker != gameState.WON) {
            turnScreens[turnTracker].SetActive(true);
        };
    }

    public void startTurn(int i) {
        PlayerController player = turnList[i];
        SubmarineLogicScript logicScript = player.gameObject.GetComponentInChildren<SubmarineLogicScript>();
        UIManager buttonManager = UIList[i].GetComponent<UIManager>();
        UIList[i].SetActive(true);
        UIList[(i + 1) % turnList.Length].SetActive(false);
        if (logicScript.getEnergy() >= silenceEnergyCost) {
            buttonManager.getSilenceButton().GetComponent<Button>().enabled = true;
        } else {
            buttonManager.getSilenceButton().GetComponent<Button>().enabled = false;
        }
        if (logicScript.getEnergy() >= torpedoEnergyCost) {
            buttonManager.getTorpedoButton().GetComponent<Button>().enabled = true;
        } else {
            buttonManager.getTorpedoButton().GetComponent<Button>().enabled = false;
        }
        if (logicScript.getEnergy() >= sonarEnergyCost) {
            buttonManager.getSonarButton().GetComponent<Button>().enabled = true;
        } else {
            buttonManager.getSonarButton().GetComponent<Button>().enabled = false;
        }
        tilemaps[i].GetComponent<TilemapRenderer>().enabled = true;
        tilemaps[(i + 1) % 2].GetComponent<TilemapRenderer>().enabled = true;
        player.gameObject.SetActive(true);
        turnList[(i + 1) % turnList.Length].gameObject.SetActive(false);
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
