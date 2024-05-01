using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.PackageManager;
using UnityEditor.Rendering;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class RadioOperator : MonoBehaviour
{
    Path path;
    
    Vector2Int[] islands;
    public GameLogicManager gameLogicManager;

    void Start() {
        path = gameObject.GetComponentInChildren<Path>();
    }

    public int reportMove(Vector2Int move) {
        if (path.nodeCount == 0) {
            path.startPath(move, new NumberGrid());
            path.updateDisplay(move, new Vector2Int((int)path.transform.position.x, (int)path.transform.position.y));
        } else {
            Path.Node[] tails = path.getTails();
            int tailsLength = tails.Length;
            for (int i = 0; i < tailsLength; i++) { // for each tail in path.tails
                Path.Node tail = tails[i];
                Vector2Int oldPosition;
                if (path.getStartingPosition() == Vector2Int.zero) {
                    oldPosition = new Vector2Int((int)path.transform.position.x, (int)path.transform.position.y) + tail.getRelativePosition();
                } else {
                    oldPosition = path.getStartingPosition() + tail.getRelativePosition();
                }
                Vector2Int targetRelativePosition = new Vector2Int(oldPosition.x + move.x, oldPosition.y + move.y);
                if (tail.getGrid() == null) {
                    tail.setGrid(new NumberGrid());
                }
                if (tail.getGrid().isFull() || path.isCollision(targetRelativePosition, tail)) {
                    path.collapseBranch(tail);
                } else {
                    Path.Node newTail = path.extendTail(i, move, false);
                    tail.setGrid(null);
                    path.updateDisplay(tail.getMove(), move, oldPosition, tail.getSilenceIn(), newTail.getSilenceIn());
                }
            }
        }
        return 1;
    }

    public int reportMove() {
        Path.Node[] tails = path.getTails();
        int tailsLength = tails.Length;
        foreach (Path.Node tail in tails) {
            Vector2Int lastMove = tail.getMove();
            Vector2Int[] moveList = new Vector2Int[] {new Vector2Int(lastMove.x, lastMove.y), new Vector2Int(lastMove.y, -lastMove.x), new Vector2Int(-lastMove.y, lastMove.x)};
            Vector2Int[] validMoveList = {};
            Vector2Int oldPosition;
            if (path.getStartingPosition() == Vector2Int.zero) {
                oldPosition = new Vector2Int((int)path.transform.position.x, (int)path.transform.position.y) + tail.getRelativePosition();
            } else {
                oldPosition = path.getStartingPosition() + tail.getRelativePosition();
            }
            for (int n = 0; n < 3; n++) {
                Vector2Int move = new Vector2Int(moveList[0].x,moveList[0].y);
                Vector2Int targetRelativePosition = new Vector2Int(oldPosition.x + move.x, oldPosition.y + move.y);
                if (tail.getGrid() == null) {
                    tail.setGrid(new NumberGrid());
                }
                NumberGrid gridCopy = new NumberGrid(tail.getGrid());
                gridCopy.update(targetRelativePosition);
                if (!gridCopy.isFull() && !path.isCollision(targetRelativePosition, tail)) {
                    Path.Node newTail = tail.addChild(move, true);
                    newTail.setGrid(gridCopy);
                    tails = tails.Concat(new Path.Node[] {newTail}).ToArray();
                    validMoveList.Concat(new Vector2Int[] {moveList[n]});
                }
            }
            if (validMoveList.Length == 0) {
                path.collapseBranch(tail);
            } else {
                tails = tails.Where(var => var != tail).ToArray();
                tail.setGrid(null);
                path.updateDisplay(tail.getMove(), validMoveList, oldPosition);
                foreach (Path.Node checkedTail in tails) {
                    if (checkedTail.getGrid() == null) {
                        Debug.LogError("A tail has exited reportMove() with a null grid");
                    }
                }
            }
        }
        path.setTails(tails);
        return 1;
    }

    public void reportSurface(Vector2Int position) {
        path.clearPath();
        path.setStartingPosition(position);
    }

    public void reportClassicSonar(int position, string positionType) {
        switch (positionType) {
            case "row":
                foreach (Path.Node tail in path.getTails()) {
                    int startingRow = position - tail.getRelativePosition().x;
                    for (int n = 1; n <= gameLogicManager.mapHeight; n++) {
                        if (n != startingRow) {
                            Debug.Log("Null grid? " + tail.getGrid() == null);
                            tail.getGrid().eliminateRow(n);
                        }
                    }
                }
                break;
            case "column":
                foreach (Path.Node tail in path.getTails()) {
                    int startingColumn = position + tail.getRelativePosition().y;
                    for (int n = 1; n <= gameLogicManager.mapWidth; n++) {
                        if (n != startingColumn) {
                            Debug.Log("Null grid? " + tail.getGrid() == null);
                            tail.getGrid().eliminateColumn(n);
                        }
                    }
                }
                break;
            case "default":
                break;
        }
    }

    public void reportProbe(int falsePosition, int falsePositionType, int truePosition, int truePositionType) {
        
    }

    public void reportProbe(int region, bool isThere) {
        
    }

    public void reportTorpedo(Vector2Int targetPosition, int hitRange, bool isHit) {
        // to be implemented later
    }
}