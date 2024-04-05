using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.PackageManager;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class RadioOperator : MonoBehaviour
{
    Path path;
    
    Vector2Int[] islands;
    public Tilemap tilemap;

    public int reportMove(Vector2Int move) {
        if (path == null) {
            path = new Path(move, tilemap);
        }
        Path.Node[] tails = path.getTails();
        int tailsLength = tails.Length;
        for (int i = 0; i < tailsLength; i++) { // for each tail in path.tails
            Path.Node tail = tails[i];
            Vector2Int oldPosition = tail.getRelativePosition();
            Vector2Int targetRelativePosition = new Vector2Int(oldPosition.x + move.x, oldPosition.y + move.y);
            if (tail.getGrid() == null) {
                NumberGrid.setIslands(islands);
                tail.setGrid(new NumberGrid());
            }
            tail.getGrid().update(targetRelativePosition);
            if (tail.getGrid().isFull() || path.isCollision(targetRelativePosition, tail)) {
                path.collapseBranch(tail);
            } else {
                Path.Node newTail = path.extendTail(i, move, false);
                newTail.setGrid(tail.getGrid());
                tail.setGrid(null);
                path.updateDisplay(tail.getMove(), move, oldPosition, tail.getSilenceIn(), newTail.getSilenceIn());
            }
        }
        return 1;
    }

    public int reportMove() {
        Path.Node[] tails = path.getTails();
        int tailsLength = tails.Length;
        for (int i = 0; i < tailsLength; i++) {
            Path.Node tail = tails[i];
            Vector2Int lastMove = tail.getMove();
            Vector2Int[] moveList = new Vector2Int[] {new Vector2Int(lastMove.x, lastMove.y), new Vector2Int(lastMove.y, -lastMove.x), new Vector2Int(-lastMove.y, lastMove.x)};
            Vector2Int[] validMoveList = {};
            Vector2Int oldPosition = tail.getRelativePosition();
            for (int n = 0; n < 3; n++) {
                Vector2Int move = new Vector2Int(moveList[0].x,moveList[0].y);
                Vector2Int targetRelativePosition = new Vector2Int(oldPosition.x + move.x, oldPosition.y + move.y);
                if (tail.getGrid() == null) {
                    NumberGrid.setIslands(islands);
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
                path.updateDisplay(tail.getMove(), validMoveList, oldPosition, tail.getSilenceIn());
            }
        }
        path.setTails(tails);
        return 1;
    }
}