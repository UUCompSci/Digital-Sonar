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
        private int[] move;
        private bool silenceIn;
        private bool silenceOut;
        private int[] relativePosition;
        public Node(Node parent, int[] move) {
            this.parent = parent;
            this.move = move;
            if (parent != null) {
                int[] temp= {parent.relativePosition[0] + move[0], parent.relativePosition[1] + move[1]};
                relativePosition = temp;
            } else {
                relativePosition = move;
            }
            children = new Node[0];
        }

        public void addChild(Node child) {
            Node[] temp = new Node[children.Length + 1];
            Array.Copy(children, 0, temp, 0, children.Length);
            temp[children.Length] = child;
            children = temp;
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

        public int[] getRelativePosition() {
            return relativePosition;
        }

        public int[] getMove() {
            return move;
        }

        public bool getSilenceIn() {
            return silenceIn;
        }

        public bool getSilenceOut() {
            return silenceOut;
        }
    }

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

    public Path(int[] move) {
        head = new Node(null, move);
        tails = new Node[1] {head};
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

    public bool isCollision(int[] position, Node tail) {
        return isCollisionHelper(tail, position);
    }

    private bool isCollisionHelper(Node node, int[] position) {
        if (node.getRelativePosition() == position) {
            return true;
        } else if (node.getParent() == null) {
            return false;
        }
        return isCollisionHelper(node.getParent(), position);
    }

    public void updateDisplaySimple(int[] lastMove, int[] move, int[] position, bool fromSilence) {
        int lastMoveInt = lastMove[0] + (lastMove[0] / 2) + lastMove[1] / 2 * 2;
        int moveInt = move[0] + (move[0] / 2) + move[1] / 2 * 2;
        Tile tile;
        if (lastMove == move) {
            tile = fromSilence ? straightSilenceIn : straight;
        } else {
            tile = fromSilence ? cornerSilenceIn : corner;
        }
        tilemap.SetTile(new Vector3Int(position[0], position[1], 0), tile);
        int shouldFlip = (moveInt + 2 - lastMoveInt) % 4 % 3 % 2; //checks whether the corner is oriented clockwise or counter-clockwise. Counter-clockwise (the default sprite direction) gives 0, and clockwise gives 1
        int quarterTurns = (4 - ((lastMoveInt + 2) % 4)) % 4; //checks how many counter-clockwise turns it needs from the default 
        tilemap.SetTransformMatrix(new Vector3Int(position[0], position[1], 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlip, 90 * quarterTurns), Vector3.one));
        Vector3Int targetPosition = new Vector3Int(position[0] + ((-1)^(moveInt / 2)) * (moveInt % 2), position[1] + ((-1)^(moveInt / 2)) * ((moveInt + 1) % 2), 0);
        tilemap.SetTile(targetPosition, endpoint);
        tilemap.SetTransformMatrix(targetPosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns), Vector3.one));
    }

    public void updateDisplaySplit(int lastMove, int[] moves, int[] position) {
        Tile tile;
        int shouldFlip = 0;
        if (!moves.Contains<int>(lastMove)) {
            tile = forking;
        } else if (moves.Length == 3) {
            tile = threeWay;
        } else {
            tile = branching;
            shouldFlip = (Math.Abs(moves[0] + moves[1] - 2 * lastMove) + 2 - lastMove) % 4 % 3 % 2;
        }
        tilemap.SetTile(new Vector3Int(position[0], position[1], 0), tile);
        tilemap.SetTransformMatrix(new Vector3Int(position[0], position[1], 0), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlip, 90 * quarterTurns(lastMove)), Vector3.one));
        for (int i = 0; i < moves.Length; i++) {
            Vector3Int targetPosition = new Vector3Int(position[0] + ((-1)^(moves[i] / 2)) * (moves[i] % 2), position[1] + ((-1)^(moves[i] / 2)) * ((moves[i] + 1) % 2), 0);
            tilemap.SetTile(targetPosition, silenceEndpoint);
            tilemap.SetTransformMatrix(targetPosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(moves[i])), Vector3.one));
        }
    }

    public int quarterTurns(int move) {
        return (4 - ((move + 2) % 4)) % 4;
    }
}