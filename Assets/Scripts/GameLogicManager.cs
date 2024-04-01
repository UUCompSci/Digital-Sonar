using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject player1Submarine;
    public GameObject player2Submarine;
    public RadioOperator player1RadioOperator;
    public RadioOperator player2RadioOperator;


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
}
