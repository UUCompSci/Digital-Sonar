using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Timeline;

public class GameLogicManager : MonoBehaviour
{
    public GameObject player1Submarine;
    public GameObject player2Submarine;
    public RadioOperator player1RadioOperator;
    public RadioOperator player2RadioOperator;
    public int mapWidth;
    public int mapHeight;
    public bool energyGainOnSilence;
    public int silenceEnergyCost;
    public int roundCount;
    private int turnTracker;
    public GameObject[] turnScreens;
    private PlayerController[] turnList;
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

    public bool gainEnergyOnSilence() {
        return energyGainOnSilence;
    }

    public int getSilenceEnergyCost() {
        return silenceEnergyCost;
    }
}
