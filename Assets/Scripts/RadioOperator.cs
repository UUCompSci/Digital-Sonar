using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class RadioOperator : MonoBehaviour
{
    Path path;
    
    Vector2Int[] islands;

    public int reportMove(Vector2Int move) {
        if (path == null) {
            path = new Path(move);
        }
        Path.Node[] tails = path.getTails();
        int tailsLength = tails.Length;
        for (int i = 0; i < tailsLength; i++) { // for each tail in path.tails
            Path.Node tail = tails[i];
            Vector2Int oldPosition = tail.getRelativePosition();
            Vector2Int targetRelativePosition = new Vector2Int(oldPosition[0] + move[0], oldPosition[1] + move[1]);
            bool fullGrid = false;
            if (fullGrid || path.isCollision(targetRelativePosition, tail)) {
                path.collapseBranch(tail);
            } else {
                //add child and give it grid
            }
        }
        return 1;
    }

    public int reportMove(string silenceMessage) {
        Path.Node[] tails = path.getTails();
        int tailsLength = tails.Length;
        for (int i = 0; i < tailsLength; i++) {
            Path.Node tail = tails[i];
            Vector2Int lastMove = tail.getMove();
            Vector2Int[] moveList = new Vector2Int[] {new Vector2Int(lastMove.x, lastMove.y), new Vector2Int(lastMove.y, -lastMove.x), new Vector2Int(-lastMove.y, lastMove.x)};
            bool branchesAdded = false;
            for (int n = 0; n < 3; n++) {
                Vector2Int move = new Vector2Int(moveList[0].x,moveList[0].y);
                Vector2Int oldPosition = tail.getRelativePosition();
                Vector2Int targetRelativePosition = new Vector2Int(oldPosition[0] + move[0], oldPosition[1] + move[1]);
                NumberGrid grid = tail.getGrid();
                NumberGrid gridCopy = new NumberGrid(grid);
                gridCopy.update(targetRelativePosition);
                if (!gridCopy.isFull() && !path.isCollision(targetRelativePosition, tail)) {
                    tail.addChild(move, true);
                    tail.setSilenceOut(true);
                    // give grid copy to child
                    branchesAdded = true;
                }
            }
            if (!branchesAdded) {
                path.collapseBranch(tail);
            }
        }
        return 1;
    }
}