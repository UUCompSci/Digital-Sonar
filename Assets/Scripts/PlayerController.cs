using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    public int moveRange;
    public int silenceRange;
    public LayerMask whatStopsMovement;

    public Transform canvas;

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

    public Vector2Int Move(SubmarineLogicScript submarine, Vector2Int targetPosition) {
        Vector2Int move = new Vector2Int(targetPosition.x - Convert.ToInt32(gameObject.transform.position[0]), targetPosition.y - Convert.ToInt32(gameObject.transform.position[0]));
        if (validateMove(targetPosition)) {
            movePoint.position = new Vector3Int(targetPosition[0], targetPosition[1], 0);
            if (submarine.getPath() != null) {
                submarine.getPath().getTails()[0].addChild(move, false);
            } else {
                submarine.setPath(move);
            }
        }
        return move;
        
    }

    public string Move(SubmarineLogicScript submarine, Vector2Int targetPosition, string report) {
        Vector2Int move = new Vector2Int(targetPosition.x - Convert.ToInt32(gameObject.transform.position[0]), targetPosition.y - Convert.ToInt32(gameObject.transform.position[0]));
        if (validateMove(targetPosition)) {
            movePoint.position = new Vector3Int(targetPosition.x, targetPosition.y, 0);
            if (submarine.getPath() != null) {
                submarine.getPath().getTails()[0].addChild(move, true);
            } else {
                submarine.setPath(move);
            }
        }
        return report;
    }

    public bool validateMove(Vector2 targetPosition) {
        return !Physics2D.OverlapCircle(new Vector3(targetPosition[0], targetPosition[1], 0), .2f, whatStopsMovement);
    }

    public void displayMoveOptions() {
        Vector2Int position = new Vector2Int(Convert.ToInt32(gameObject.transform.position.x), Convert.ToInt32(gameObject.transform.position.y));
        for (int move = 0; move <= 3; move++) {
            UnityEngine.Debug.Log(move.ToString());
            for (int distance = 1; distance <= moveRange; distance++) {
                Vector2Int targetPosition = new Vector2Int(position.x + (int)Math.Pow(-1, move / 2) * (move % 2) * distance, position.y + (int)Math.Pow(-1, move / 2) * ((move + 1) % 2) * distance);
                bool isValid = validateMove(targetPosition); // same mod operation to check for the sign and floor division (not inverted this time) to check if the direction is supposed to be vertical
                if (!isValid) {
                    distance += moveRange; // ends the loop early on hitting an obstacle
                } else {
                    UnityEngine.Debug.Log("Creating move target at" + targetPosition.ToString());
                    GameObject tempMoveTarget = Instantiate(moveTarget, new Vector3Int(targetPosition.x, targetPosition.y, 0), Quaternion.Euler(Vector3.zero), canvas);
                    tempMoveTarget.name = "MoveTarget";
                }
                UnityEngine.Debug.Log("Distance: " + distance.ToString());
                UnityEngine.Debug.Log("TargetPosition: " + targetPosition.ToString());
            }
        }
    }

    public void displaySilenceOptions() {
        Vector2Int position = new Vector2Int(Convert.ToInt32(gameObject.transform.position[0]), Convert.ToInt32(gameObject.transform.position[1]));
        for (int move = 0; move <= 3; move++) {
            UnityEngine.Debug.Log(move);
            for (int distance = 1; distance <= silenceRange; distance++) {
                Vector2Int targetPosition = new Vector2Int(position.x + (int)Math.Pow(-1, move / 2) * (move % 2) * distance, position.y + (int)Math.Pow(-1, move / 2) * ((move + 1) % 2) * distance);
                bool isValid = validateMove(targetPosition); // same mod operation to check for the sign and floor division (not inverted this time) to check if the direction is supposed to be vertical
                distance += silenceRange * Convert.ToInt32(!isValid); // ends the loop early on hitting an obstacle
                if (isValid) {
                    UnityEngine.Debug.Log("Creating move target at" + targetPosition.ToString());
                    GameObject tempSilenceTarget = Instantiate(silenceTarget, new Vector3Int(targetPosition[0], targetPosition[1], 0), Quaternion.Euler(Vector3.zero), canvas);
                    tempSilenceTarget.name = "silenceTarget";
                }
            }
        }
    }

    public void clearCanvas() {
        for (int i = 0; i < canvas.childCount; i++) {
            Destroy(canvas.GetChild(i).gameObject);
        }
    }
}
