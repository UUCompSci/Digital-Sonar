using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    GameObject gameboard = GameObject.Find("Grid");
    
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

}
