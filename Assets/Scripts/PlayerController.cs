using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TerrainTools;
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
    public int whatStopsMovement;

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
        Vector2Int move = new Vector2Int(targetPosition.x - (int)gameObject.transform.position.x, targetPosition.y - (int)gameObject.transform.position.y);
        if (submarine.getPath() != null) {
            Path.Node[] tails = submarine.getPath().getTails();
            tails[0] = tails[0].addChild(move, false);
        } else {
            submarine.setPath(move);
        }
        movePoint.position = new Vector3Int(targetPosition.x, targetPosition.y, 0);
        return move;
    }

    public string Move(SubmarineLogicScript submarine, Vector2Int targetPosition, string report) {
        Vector2Int move = new Vector2Int(targetPosition.x - (int)gameObject.transform.position.x, targetPosition.y - (int)gameObject.transform.position.y);
        if (validateMove(move)) {
            movePoint.position = new Vector3Int(targetPosition.x, targetPosition.y, 0);
            if (submarine.getPath() != null) {
                submarine.getPath().getTails()[0].addChild(move, true);
            } else {
                submarine.setPath(move);
            }
        }
        return report;
    }

    public bool validateMove(Vector2Int move) {
        Vector2Int targetPosition = new Vector2Int((int)transform.position.x + move.x, (int)transform.position.y + move.y);
        Path path = gameObject.GetComponentInChildren<SubmarineLogicScript>().getPath();
        bool result2 = false;
        if (path != null) {
            Path.Node tail = path.getTails()[0];
            result2 = path.isCollision(new Vector2Int(targetPosition.x - path.getStartingPosition().x, targetPosition.y - path.getStartingPosition().y), tail);
        }
        Tile tile = (Tile)islandMap.GetTile(islandMap.WorldToCell(new Vector3Int(targetPosition.x, targetPosition.y, 0)));
        bool result3 = !(result2 || (tile != null && tile == island));
        return result3;
    }

    public void displayMoveOptions() {
        Vector2Int position = new Vector2Int(Convert.ToInt32(gameObject.transform.position.x), Convert.ToInt32(gameObject.transform.position.y));
        for (int dir = 0; dir <= 3; dir++) {
            for (int dist = 1; dist <= moveRange; dist++) {
                Vector2Int move = new Vector2Int((int)Math.Pow(-1, dir / 2) * (dir % 2) * dist, (int)Math.Pow(-1, dir / 2) * ((dir + 1) % 2) * dist);
                Vector2Int targetPosition = new Vector2Int(position.x + move.x, position.y + move.y);
                bool isValid = validateMove(move); 
                if (!isValid) {
                    dist += moveRange; // ends the loop early on hitting an obstacle
                } else {
                    GameObject tempMoveTarget = Instantiate(moveTarget, new Vector3Int(targetPosition.x, targetPosition.y, 0), Quaternion.Euler(Vector3.zero), canvas);
                    tempMoveTarget.name = "MoveTarget";
                }
            }
        }
    }

    public void displaySilenceOptions() {
        Vector2Int position = new Vector2Int(Convert.ToInt32(gameObject.transform.position.x), Convert.ToInt32(gameObject.transform.position[1]));
        for (int move = 0; move <= 3; move++) {
            UnityEngine.Debug.Log(move);
            for (int dist = 1; dist <= silenceRange; dist++) {
                Vector2Int targetPosition = new Vector2Int(position.x + (int)Math.Pow(-1, move / 2) * (move % 2) * dist, position.y + (int)Math.Pow(-1, move / 2) * ((move + 1) % 2) * dist);
                bool isValid = validateMove(targetPosition); // same mod operation to check for the sign and floor division (not inverted this time) to check if the dir is supposed to be vertical
                dist += silenceRange * Convert.ToInt32(!isValid); // ends the loop early on hitting an obstacle
                if (isValid) {
                    GameObject tempSilenceTarget = Instantiate(silenceTarget, new Vector3Int(targetPosition.x, targetPosition.y, 0), Quaternion.Euler(Vector3.zero), canvas);
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
