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
    
    int[,] islands;

    public int reportMove(int[] move) {
        Path.Node[] tails = path.getTails();
        int tailsLength = tails.Length;
        for (int i = 0; i < tailsLength; i++) { // for each tail in path.tails
            Path.Node tail = tails[i];
            int[] oldPosition = tail.getRelativePosition();
            int[] targetRelativePosition = {oldPosition[0] + move[0], oldPosition[1] + move[1]};
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
            int[] lastMove = tail.getMove();
            int[,] moveList = {{lastMove[0], lastMove[1]}, {lastMove[1], -lastMove[0]}, {-lastMove[1], lastMove[0]}};
            bool branchesAdded = false;
            for (int n = 0; n < 3; n++) {
                int[] move = {moveList[0,0],moveList[0,1]};
                int[] oldPosition = tail.getRelativePosition();
                int[] targetRelativePosition = {oldPosition[0] + move[0], oldPosition[1] + move[1]};
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