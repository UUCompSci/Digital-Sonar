using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class RadioOperator : MonoBehavior
{
    Path path;
    
    int[,] islands;

    int reportMove(int[] move) {
        tails = path.getTails();
        tailsLength = tails.Length;
        for (int i = 0; i < tailsLength; i++) { // for each tail in path.tails
            Node tail = tails[i];
            int[] oldPosition = tail.getRelativePosition();
            int[] targetPosition = [oldPosition[0] + move[0], oldPosition[1] + move[1]];
            bool fullGrid;
            if (fullGrid || Path.isCollision(targetRelativePosition, tail)) {
                path.collapseBranch(tail);
            } else {
                //add child and give it grid
            }
        }
    }

    int reportMove(char silenceMessage) {
        Node[] tails = path.getTails();
        int tailsLength = tails.Length;
        for (int i = 0; i < tailsLength; i++) {
            Node tail = tails[i]
            int[] lastMove = tail.getMove()
            int[,] moveList = [[lastMove[0], lastMove[1]], [lastMove[1], -lastMove[0]], [-lastMove[1], lastMove[0]]];
            bool branchesAdded = false;
            for (int i = 0; i < 3; i++) {
                int[] move[] = moveList[0]
                int[] oldPosition = tail.getRelativePosition();
                int[] targetRelativePosition = [oldPosition[0] + move[0], oldPosition[1] + move[1]]
                // copy grid and update copy
                bool fullGrid; //check if COPY is full
                if (!fullGrid && !path.isCollision(targetRelativePosition, tail)) {
                    tail.addChild(path.Node.Node(tail, move, true));
                    tail.setSilenceOut(true);
                    // give grid copy to child
                    branchesAdded = true;
                }
            }
            if (!branchesAdded) {
                path.collapseBranch(tail);
            }
        }
    }

    public grid updateGrid(relativePosition) {
        for (int i = 0; i < islands.Length; i++) {
            // eliminate [islands[i][0] - relativePosition[0], islands[i][1] - relativePosition[1]] as a possible starting location
        }
    }

    public bool isGridFull(grid) {
        for (int x = 1; x <= grid[0].Length; x++) {
            for (int y = 1; y <= grid.Length; y++) {
                if (grid[x,y] == 0) {
                    return false;
                };
            };
        };
        return true;
    }
}