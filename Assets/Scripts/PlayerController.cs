using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Transform movePoint;
    public Tilemap islandMap;
    public Tile island;
    public float moveSpeed;
    public GameObject moveTarget;
    public GameObject silenceTarget;
    public int moveRange;
    public int silenceRange;
    public int whatStopsMovement;
    public Transform canvas;
    public GameLogicManager gameLogicManager;
    public RadioOperator reportee;
    public GameObject minePrefab;
    public Canvas classicSonarPrompt;
    public Canvas torpedoMenu;


    // Start is called before the first frame update
    void Start()
    {
        movePoint.SetParent(null);
        gameLogicManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>();
        reportee = gameLogicManager.getOpponentRadioOperator(this);
        classicSonarPrompt.gameObject.transform.SetParent(gameLogicManager.getUIManager(this).transform);
        classicSonarPrompt.gameObject.SetActive(false);
        torpedoMenu.gameObject.GetComponentInChildren<TorpedoMenu>().parentSub = this;
        torpedoMenu.gameObject.transform.SetParent(null);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, gameObject.GetComponentInChildren<SubmarineLogicScript>().moveSpeed * Time.deltaTime);
    }

    public Vector2Int Move(SubmarineLogicScript submarine, Vector2Int targetPosition) {
        Vector2Int move = new Vector2Int(targetPosition.x - (int)transform.position.x, targetPosition.y - (int)transform.position.y);
        if (submarine.getPath().nodeCount != 0) {
            Path.Node[] tails = submarine.getPath().getTails();
            submarine.getPath().extendTail(0, move, false);
            submarine.getPath().updateDisplay(tails[0].getParent().getMove(), move, new Vector2Int((int)transform.position.x, (int)transform.position.y), tails[0].getSilenceIn(), false);
        } else {
            submarine.setPath(move, new Vector2Int((int)transform.position.x, (int)transform.position.y));
            submarine.getPath().updateDisplay(move, new Vector2Int((int)transform.position.x, (int)transform.position.y));
        }
        movePoint.position = new Vector3Int(targetPosition.x, targetPosition.y, 0);
        return move;
    }

    public string Move(SubmarineLogicScript submarine, Vector2Int targetPosition, string report) {
        Vector2Int move = new Vector2Int(targetPosition.x - (int)transform.position.x, targetPosition.y - (int)transform.position.y);
        if (submarine.getPath().nodeCount != 0) {
            Path.Node[] tails = submarine.getPath().getTails();
            submarine.getPath().extendTail(0, move, true);
            submarine.getPath().updateDisplay(tails[0].getParent().getMove(), move, new Vector2Int((int)transform.position.x, (int)transform.position.y), tails[0].getSilenceIn(), true);
        }
        movePoint.position = new Vector3Int(targetPosition.x, targetPosition.y, 0);
        return report;
    }

    public bool validateMove(Vector2Int move) {
        Vector2Int targetPosition = new Vector2Int((int)movePoint.position.x + move.x, (int)movePoint.position.y + move.y);
        Path path = gameObject.GetComponentInChildren<Path>();
        bool result2 = false;
        if (path.nodeCount != 0) {
            Path.Node tail = path.getTails()[0];
            result2 = path.isCollision(new Vector2Int(targetPosition.x - path.getStartingPosition().x, targetPosition.y - path.getStartingPosition().y), tail);
        }
        Tile tile = (Tile)islandMap.GetTile(islandMap.WorldToCell(new Vector3Int(targetPosition.x, targetPosition.y, 0)));
        bool result3 = !(result2 || (tile != null && tile == island));
        return result3;
    }

    public void displayMoveOptions() {
        Vector2Int position = new Vector2Int(Convert.ToInt32(movePoint.position.x), Convert.ToInt32(movePoint.position.y));
        for (int dir = 0; dir <= 3; dir++) {
            for (int dist = 1; dist <= moveRange; dist++) {
                Vector2Int move = new Vector2Int((int)Math.Pow(-1, dir / 2) * (dir % 2) * dist, (int)Math.Pow(-1, dir / 2) * ((dir + 1) % 2) * dist);
                Vector2Int targetPosition = new Vector2Int(position.x + move.x, position.y + move.y);
                bool isValid = validateMove(move); 
                dist += moveRange * Convert.ToInt32(!isValid); // ends the loop early on hitting an obstacle
                if (isValid) {
                    GameObject newMoveTarget = Instantiate(moveTarget, new Vector3Int(targetPosition.x, targetPosition.y, 0), Quaternion.Euler(Vector3.zero), canvas);
                    newMoveTarget.name = "MoveTarget";
                }
            }
        }
    }

    public void displaySilenceOptions() {
        Vector2Int position = new Vector2Int(Convert.ToInt32(movePoint.position.x), Convert.ToInt32(movePoint.position.y));
        for (int dir = 0; dir <= 3; dir++) {
            for (int dist = 1; dist <= silenceRange; dist++) {
                Vector2Int move = new Vector2Int((int)Math.Pow(-1, dir / 2) * (dir % 2) * dist, (int)Math.Pow(-1, dir / 2) * ((dir + 1) % 2) * dist);
                Vector2Int targetPosition = new Vector2Int(position.x + move.x, position.y + move.y);
                bool isValid = validateMove(move); // same mod operation to check for the sign and floor division (not inverted this time) to check if the dir is supposed to be vertical
                dist += silenceRange * Convert.ToInt32(!isValid); // ends the loop early on hitting an obstacle
                if (isValid) {
                    GameObject newSilenceTarget = Instantiate(silenceTarget, new Vector3Int(targetPosition.x, targetPosition.y, 0), Quaternion.Euler(Vector3.zero), canvas);
                    newSilenceTarget.name = "silenceTarget";
                }
            }
        }
    }

    public void clearCanvas() {
        for (int i = 0; i < canvas.childCount; i++) {
            Destroy(canvas.GetChild(i).gameObject);
        }
    }

    public void resolveClassicSonar(int positionType) {
        switch (positionType) {
            case 0:
                reportee.reportClassicSonar((int)transform.position.x + 5, "row");
                break;
            case 1:
                reportee.reportClassicSonar((int)transform.position.y + 6, "column");
                break;
            default:
                break;
        }
    }

    public void surface() {
        gameLogicManager.spendAction(this);
        clearCanvas();
        gameObject.GetComponentInChildren<SubmarineLogicScript>().clearPath();
        reportee.reportSurface(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }
}
