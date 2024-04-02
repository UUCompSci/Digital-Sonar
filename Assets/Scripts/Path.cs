using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Path {
    public class Node {
        private Node parent;
        private Node[] children;
        private Vector2Int move;
        private bool silenceIn;
        private bool silenceOut;
        private Vector2Int relativePosition;

        private NumberGrid grid;
        // note to self: add grid for radio operator
        public Node(Node parent, Vector2Int move) {
            this.parent = parent;
            this.move = move;
            if (parent != null) {
                Vector2Int temp= new Vector2Int(parent.relativePosition.x + move.x, parent.relativePosition.y + move.y);
                relativePosition = temp;
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
                Vector2Int temp= new Vector2Int(parent.relativePosition.x + move.x, parent.relativePosition.y + move.y);
                relativePosition = temp;
            } else {
                relativePosition = move;
            }
            children = new Node[0];
        }

        public Node addChild(Vector2Int move, bool silenceIn) {
            Debug.Log("Adding child node with move " + move + " to parent node with move " + this.move);
            Node[] temp = new Node[children.Length + 1];
            Array.Copy(children, 0, temp, 0, children.Length);
            silenceOut = silenceIn;
            Node newChild = new Node(this, move, silenceIn);
            temp[children.Length] = newChild;
            children = temp;
            return newChild;
        }
        
        public void removeChild(Node child) {
            Node[] temp = new Node[children.Length - 1];
            temp = children.Where(val => val != child).ToArray();
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
    public Tile silenceEndpoint;

    public Path(Vector2Int move) {
        head = new Node(null, Vector2Int.zero);
        tails = new Node[1] {new Node(head, move)};
    }
    public Path(Vector2Int move, Vector2Int startingPosition) {
        this.startingPosition = startingPosition;
        head = new Node(null, Vector2Int.zero);
        tails = new Node[1] {new Node(head, move)};
    }

    public Vector2Int getStartingPosition() {
        return startingPosition;
    }
    public Node[] getTails() {
        return tails;
    }

    public void collapseBranch(Node tail) {
        if (tail.getParent() == null) {
            throw new Exception("Can't collapse a branch on a branchless path.");
        } else if (tail.getParent().getChildren().Length <= 1) {
            collapseBranch(tail.getParent());
        } else {
            tail.getParent().removeChild(tail);
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

    public void updateDisplaySimple(Vector2Int lastMove, Vector2Int move, Vector2Int position, bool fromSilence) {
        int lastMoveInt = lastMove[0] + (lastMove[0] / 2) + lastMove[1] / 2 * 2;
        Tile tile;
        if (lastMove == move) {
            tile = fromSilence ? straightSilenceIn : straight;
        } else {
            tile = fromSilence ? cornerSilenceIn : corner;
        }
        tilemap.SetTile(new Vector3Int(position[0], position[1], 0), tile);
        int quarterTurns = (lastMove[0] + 2) * (lastMove[0] % 2) + (lastMove[1] + 3) % 4 % 3; //checks how many counter-clockwise turns it needs from the default 
        tilemap.SetTransformMatrix(new Vector3Int(position[0], position[1], 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlip(lastMove, move), 90 * quarterTurns), Vector3.one));
        Vector3Int targetPosition = new Vector3Int(position[0] + move[0], position[1] + move[1], 0);
        tilemap.SetTile(targetPosition, endpoint);
        tilemap.SetTransformMatrix(targetPosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns), Vector3.one));
    }

    public void updateDisplaySplit(Vector2Int lastMove, Vector2Int[] moves, Vector2Int position) {
        Tile tile;
        int shouldFlipBranch = 0;
        if (moves.Length == 3) {
            tile = threeWay;
        } else if (!((moves[0].x == lastMove[0] && moves[0].y == lastMove[1]) || (moves[1].x == lastMove[0] && moves[1].y == lastMove[1]))) {
            tile = forking;
        } else {
            tile = branching;
            shouldFlipBranch = shouldFlip(lastMove, moves);
        }
        tilemap.SetTile(new Vector3Int(position[0], position[1], 0), tile);
        tilemap.SetTransformMatrix(new Vector3Int(position[0], position[1], 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlipBranch, 90 * quarterTurns(lastMove)), Vector3.one));
        for (int i = 0; i < moves.Length; i++) {
            Vector3Int targetPosition = new Vector3Int(position[0] + moves[i].x, position[1] + moves[i].y, 0);
            tilemap.SetTile(targetPosition, silenceEndpoint);
            tilemap.SetTransformMatrix(targetPosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(new Vector2Int(moves[i].x, moves[i].y))), Vector3.one));
        }
    }

    public int quarterTurns(Vector2Int move) {
        return (move[0] + 2) * (move[0] % 2) + (move[1] + 3) % 4 % 3;
    }

    public int shouldFlip(Vector2Int lastMove, Vector2Int move) {
        return (move[1] + lastMove[0] + 3) % 4 / 3 * ((move[0] - lastMove[1] + 3) % 4 / 3); // math based on matrix for clockwise rotation about the origin
    }

    public int shouldFlip(Vector2Int lastMove, Vector2Int[] moves) {
        return ((moves[0].y + lastMove[0] + 3) % 4 / 3 * ((moves[0].x - lastMove[1] + 3) % 4 / 3)) + ((moves[1].y + lastMove[0] + 3) % 4 / 3 * ((moves[1].x - lastMove[1] + 3) % 4 / 3)); // math based on matrix for clockwise rotation about the origin
    }
}