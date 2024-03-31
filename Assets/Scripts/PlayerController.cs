using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Transform movePoint;
    public Tilemap islandMap;
    public Tile island;
    public float moveSpeed;
    public RadioOperator reportee;
    public GameObject moveTarget;
    public GameObject silenceTarget;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, gameObject.GetComponentInChildren<SubmarineLogicScript>().moveSpeed * Time.deltaTime);
    }

    public int[] Move(SubmarineLogicScript submarine, int[] targetPosition) {
        int[] move = {targetPosition[0] - Convert.ToInt32(gameObject.transform.position[0]), targetPosition[1] - Convert.ToInt32(gameObject.transform.position[0])};
        if (validateMove(targetPosition)) {
            movePoint.position = new Vector3Int(targetPosition[0], targetPosition[1], 0);
            submarine.getPath().getTails()[0].addChild(move, false);
        }
        return move;
        
    }

    public string Move(SubmarineLogicScript submarine, int[] targetPosition, string report) {
        int[] move = {targetPosition[0] - Convert.ToInt32(gameObject.transform.position[0]), targetPosition[1] - Convert.ToInt32(gameObject.transform.position[0])};
        if (validateMove(targetPosition)) {
            movePoint.position = new Vector3Int(targetPosition[0], targetPosition[1], 0);
            submarine.getPath().getTails()[0].addChild(move, true);
        }
        return report;
    }

    public bool validateMove(int[] targetPosition) {
        Path path = gameObject.GetComponentInChildren<SubmarineLogicScript>().getPath();
        return !(islandMap.GetTile(new Vector3Int(targetPosition[0], targetPosition[1], 0)).name == "Island Tile"|| path.isCollision(targetPosition, path.getTails()[0]));
    }

    void displayMoveOptions(int range, bool silence) {
        int[] position = new int[] {Convert.ToInt32(gameObject.transform.position[0]), Convert.ToInt32(gameObject.transform.position[1])};
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
                    if (!silence) {
                        GameObject tempMoveTarget = Instantiate(moveTarget, new Vector3Int(targetPosition[0], targetPosition[1], 0), Quaternion.Euler(Vector3.zero), gameObject.transform);
                        tempMoveTarget.GetComponent<ReportMoveTarget>().reportee = reportee;
                        tempMoveTarget.name = "moveTarget";
                    } else {
                        GameObject tempSilenceTarget = Instantiate(silenceTarget, new Vector3Int(targetPosition[0], targetPosition[1], 0), Quaternion.Euler(Vector3.zero), gameObject.transform);
                        tempSilenceTarget.GetComponent<SilenceMoveTarget>().reportee = reportee;
                        tempSilenceTarget.name = "silenceTarget";
                    }
                }
            }
        }
    }
}
