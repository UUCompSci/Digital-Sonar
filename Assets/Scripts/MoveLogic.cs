using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    GameObject gameboard = GameObject.Find("Grid");

    bool playerMoveTurn;
    
    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = gameboard.GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int Move(SubmarineLogicScript submarine, int range, int[] move, bool report) {
        int[] targetPosition = {submarine.getPosition()[0] + move[0] * range, submarine.getPosition()[1] + move[1] * range};
        if (validateMove(targetPosition)) {
            gameObject.GetComponent<SubmarineLogicScript>().updatePosition(move, report);
            // prompt animation
        }
        int direction = move[0] + (move[0] / 2) + move[1] / 2 * 2;
        return report ? direction : -1;
    }

    public bool validateMove(int[] targetPosition) {
        Path path = gameObject.GetComponent<SubmarineLogicScript>().getPath();
        return !(gameboard.GetComponent<Tilemap>().GetTile(new Vector3Int(targetPosition[0], targetPosition[1], 0))|| path.isCollision(targetPosition, path.getTails()[0]));
    }

    void displayMoveOptions(int range) {
        int[] position = GetComponent<SubmarineLogicScript>().getPosition();
        for (int move = 0; move <= 3; move++) {
            Console.WriteLine(move);
            for (int distance = 1; distance <= range; distance++) {
                int[] targetPosition = 
                    {position[0] + ((-1)^(move / 2)) * (move % 2) * distance, // uses a mod operation to check for the sign and floor division (inverted by adding 1 and taking the mod 2) to check if the direction is supposed to be horizontal
                    position[1] + ((-1)^(move / 2)) * ((move + 1) % 2) * distance};
                bool isValid = validateMove(targetPosition); // same mod operation to check for the sign and floor division (not inverted this time) to check if the direction is supposed to be vertical
                distance += range * Convert.ToInt32(!isValid); // ends the loop early on hitting an obstacle
                if (isValid) {
                    Console.WriteLine("Creating move target at" + targetPosition.ToString());
                    //create move target
                }
            }
        }
    }

}up csode