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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string Move(SubmarineLogicScript submarine, int range, char direction, bool report) {
        Path path = submarine.getPath();
        int[] move = {range * Convert.ToInt32(direction == 'N') + (-1 * Convert.ToInt32(direction == 'S')), range * Convert.ToInt32(direction == 'E' + (-1 * Convert.ToInt32(direction == 'W')))};
        int[] targetPosition = {submarine.getPosition()[0] + move[0], submarine.getPosition()[1] + move[1]};
        if (validateMove(targetPosition)) {
            gameObject.GetComponent<SubmarineLogicScript>().updatePosition(move);
            //trigger animation
        }
        return report ? direction.ToString() : "…";
    }

    public bool validateMove(int[] targetPosition) {
        Path path = gameObject.GetComponent<SubmarineLogicScript>().getPath();
        return !(gameboard.GetComponent<Tilemap>().GetTile(new Vector3Int(targetPosition[0], targetPosition[1], 0))|| path.isCollision(targetPosition, path.getTails()[0]));
    }

    void displayMoveOptions(int range) {
        int[] position = GetComponent<SubmarineLogicScript>().getPosition();
        for (int n = 0; n <= 3; n++) {
            Console.WriteLine(n);
            for (int i = 1; i <= range; i++) {
                int[] targetPosition = 
                    {position[0] + ((-1)^(n % 2)) * (((n / 2) + 1) % 2) * i, // uses a mod operation to check for the sign and floor division (inverted by adding 1 and taking the mod 2) to check if the direction is supposed to be horizontal
                    position[1] + ((-1)^(n % 2)) * (n / 2) * i};
                bool isValid = validateMove(targetPosition); // same mod operation to check for the sign and floor division (not inverted this time) to check if the direction is supposed to be vertical
                i += range * Convert.ToInt32(!isValid); // ends the loop early on hitting an obstacle
                if (isValid) {
                    Console.WriteLine("Creating move target at" + targetPosition.ToString());
                    //create move target
                }
            }
        }
    }

}
