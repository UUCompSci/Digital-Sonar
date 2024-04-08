using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Path : MonoBehaviour {
    public class Node {
        private Node parent;
        private Node[] children;
        private Vector2Int move;
        private bool silenceIn;
        private bool silenceOut;
        private Vector2Int relativePosition;
        private NumberGrid grid;
        public Node(Node parent, Vector2Int move) {
            this.parent = parent;
            this.move = move;
            if (parent != null) {
                relativePosition = new Vector2Int(parent.relativePosition.x + move.x, parent.relativePosition.y + move.y);
            } else {
                relativePosition = move;
            }
            children = new Node[0];
        }

        public Node(Node parent, Vector2Int move, bool silenceIn) {
            this.parent = parent;
            this.move = move;
            this.silenceIn = silenceIn;
            if (parent != null) {
                relativePosition = new Vector2Int(parent.relativePosition.x + move.x, parent.relativePosition.y + move.y);
            } else {
                relativePosition = move;
            }
            children = new Node[0];
        }

        public Node(Node parent, Vector2Int move, NumberGrid grid, bool silenceIn) {
            this.parent = parent;
            this.move = move;
            this.silenceIn = silenceIn;
            if (parent != null) {
                relativePosition = new Vector2Int(parent.relativePosition.x + move.x, parent.relativePosition.y + move.y);
            } else {
                relativePosition = move;
            }
            children = new Node[0];
        }

        public Node addChild(Vector2Int move, bool silenceIn) {
            Debug.Log("Adding child node with move " + move + " to parent node with move " + this.move);
            Node newChild = new Node(this, move, silenceIn);
            children = children.Concat(new Node[] {newChild}).ToArray();
            silenceOut = silenceIn;
            return newChild;
        }
        
        public void removeChild(Node child) {
            children = children.Where(val => val != child).ToArray();
        }

        public Node getParent() {
            return parent;
        }

        public Node[] getChildren() {
            return children;
        }

        public Vector2Int getRelativePosition() {
            return relativePosition;
        }

        public Vector2Int getMove() {
            return move;
        }

        public bool getSilenceIn() {
            return silenceIn;
        }

        public bool getSilenceOut() {
            return silenceOut;
        }

        public void setSilenceIn(bool silenceIn) {
            this.silenceIn = silenceIn;
        }

        public void setSilenceOut(bool silenceOut) {
            this.silenceOut = silenceOut;
        }

        public NumberGrid getGrid() {
            return grid;
        }
        public void setGrid(NumberGrid grid) {
            this.grid = grid;
        }
    }

    private Vector2Int startingPosition;
    private Node head;
    private Node[] tails;
    public int nodeCount;
    public Tilemap tilemap;
    public Tile straight;
    public Tile corner;
    public Tile endpoint;
    public Tile start;
    public Tile threeWay;
    public Tile branching;
    public Tile forking;
    public Tile straightSilenceIn;
    public Tile straightSilenceOut;
    public Tile cornerSilenceIn;
    public Tile cornerSilenceOut;
    private int collapsingBranchSize;
    public Tile silenceEndpoint;

    public void startPath(Vector2Int move) {
        head = new Node(null, Vector2Int.zero);
        tails = new Node[1] {new Node(head, move)};
        nodeCount = 2;
    }

    public Vector2Int getStartingPosition() {
        return startingPosition;
    }

    public void setStartingPosition(Vector2Int startingPosition) {
        this.startingPosition = startingPosition;
    }

    public void removeTail(Node tail) {
        tail.getParent().removeChild(tail);
        tails = tails.Where(var => var != tail).ToArray(); 
        nodeCount -= 1;
    }

    public Node extendTail(int i, Vector2Int move, bool silenceIn) {
        tails[i] = tails[i].addChild(move, silenceIn);
        nodeCount += 1;
        return tails[i];
        
    }

    public Node[] extendTail(int tailsIndex, Vector2Int[] moves) {
        Node[] newTails = {};
        for (int i = 0; i < moves.Length; i++) {
            newTails.Concat(new Node[] {tails[tailsIndex].addChild(moves[i], true)});
            nodeCount += 1;
        }
        tails = tails.Where(var => var != tails[tailsIndex]).ToArray();
        return newTails;
    }

    public void clearPath() {
        head = null;
        tails = new Node[] {};
        nodeCount = 0;
    }
    
    public void setTails(Node[] tails) {
        this.tails = tails;
    }

    public Node[] getTails() {
        return tails;
    }

    public void collapseBranch(Node tail) {
        collapsingBranchSize = 0;
        if (tail.getParent() == null) {
            throw new Exception("Can't collapse a branch on a branchless path.");
        } else if (tail.getParent().getChildren().Length <= 1) {
            collapsingBranchSize += 1;
            collapseBranchHelper(tail.getParent());
        } else {
            tail.getParent().removeChild(tail);
            removeTail(tail);
        }
    }

    private void collapseBranchHelper(Node tail) {
        if (tail.getParent() == null) {
            throw new Exception("Can't collapse a branch on a branchless path.");
        } else if (tail.getParent().getChildren().Length <= 1) {
            collapsingBranchSize += 1;
            collapseBranchHelper(tail.getParent());
        } else {
            tail.getParent().removeChild(tail);
            removeTail(tail);
            nodeCount -= collapsingBranchSize;
        }
    }

    public bool isCollision(Vector2Int relativePosition, Node tail) {
        Debug.Log("Checking for nodes with relativePosition " + relativePosition);
        return isCollisionHelper(relativePosition, tail);
    }

    private bool isCollisionHelper(Vector2Int relativePosition, Node node) {
        if (node.getRelativePosition() == relativePosition) {
            Debug.Log("Node at relative position " + relativePosition + " found");
            return true;
        } else if (node.getParent() == null) {
            Debug.Log("Head reached, no collisions found!");
            return false;
        }
        Debug.Log("Node with move " + node.getMove() + " has relative position " + node.getRelativePosition());
        return isCollisionHelper(relativePosition, node.getParent());
    }

    public void updateDisplay(Vector2Int move, Vector2Int position) {
        tilemap.SetTile(tilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), start);
        tilemap.SetTransformMatrix(tilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(move, 0)), Vector3.one));
        Vector3Int targetPosition = new Vector3Int(position.x + move.x, position.y + move.y, 0);
        tilemap.SetTile(tilemap.WorldToCell(targetPosition), endpoint);
        tilemap.SetTransformMatrix(tilemap.WorldToCell(targetPosition), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(move, 0)), Vector3.one));
    }

    public void updateDisplay(Vector2Int lastMove, Vector2Int move, Vector2Int position, bool fromSilence, bool toSilence) {
        Tile tile;
        if (lastMove == move) {
            if (fromSilence) {
                tile = straightSilenceIn;
            } else if (toSilence) {
                tile = straightSilenceOut;
            } else {
                tile = straight;
            }
        } else {
            if (fromSilence) {
                tile = cornerSilenceIn;
            } else if (toSilence) {
                tile = cornerSilenceOut;
            } else {
                tile = corner;
            }
        }
        tilemap.SetTile(tilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), tile);
        int shouldFlip = this.shouldFlip(lastMove, move);
        tilemap.SetTransformMatrix(tilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlip, 90 * quarterTurns(lastMove, shouldFlip)), Vector3.one));
        // tilemap.SetTransformMatrix(tilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlip, 90 * ((quarterTurns(lastMove) + 2 * shouldFlip) % 4)), Vector3.one));
        Vector3Int targetPosition = new Vector3Int(position.x + move.x, position.y + move.y, 0);
        tilemap.SetTile(tilemap.WorldToCell(targetPosition), endpoint);
        tilemap.SetTransformMatrix(tilemap.WorldToCell(targetPosition), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(move, 0)), Vector3.one));
    }

    public void updateDisplay(Vector2Int lastMove, Vector2Int[] moves, Vector2Int position) {
        Tile tile;
        int shouldFlipBranch = 0;
        if (moves.Length == 3) {
            tile = threeWay;
        } else if (!((moves[0].x == lastMove.x && moves[0].y == lastMove.y) || (moves[1].x == lastMove.x && moves[1].y == lastMove.y))) {
            tile = forking;
        } else {
            tile = branching;
            shouldFlipBranch = shouldFlip(lastMove, moves);
        }
        tilemap.SetTile(tilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), tile);
        tilemap.SetTransformMatrix(tilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlipBranch, 90 * ((quarterTurns(lastMove, shouldFlipBranch) + 2 * shouldFlipBranch) % 4)), Vector3.one));
        for (int i = 0; i < moves.Length; i++) {
            Vector3Int targetPosition = new Vector3Int(position.x + moves[i].x, position.y + moves[i].y, 0);
            tilemap.SetTile(tilemap.WorldToCell(targetPosition), silenceEndpoint);
            tilemap.SetTransformMatrix(tilemap.WorldToCell(targetPosition), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(new Vector2Int(moves[i].x, moves[i].y), 0)), Vector3.one));
        }
    }

    public int quarterTurns(Vector2Int move, int shouldFlip) {
        return ((move.y + 1) * (move.y % 2)) + ((2 * shouldFlip) + move.x + 2) % 4 * (move.x % 2);
    }

    public int shouldFlip(Vector2Int lastMove, Vector2Int move) {
        return (lastMove.x - move.y + 3) % 4 / 3 * ((lastMove.y + move.x + 3) % 4 / 3); // math based on matrix for clockwise rotation about the origin
    }

    public int shouldFlip(Vector2Int lastMove, Vector2Int[] moves) {
        return ((lastMove.x - moves[0].y + 3) % 4 / 3 * ((lastMove.y + moves[0].x + 3) % 4 / 3)) + ((lastMove.x - moves[1].y + 3) % 4 / 3 * ((lastMove.y + moves[1].x + 3) % 4 / 3)); // math based on matrix for clockwise rotation about the origin
    }
}