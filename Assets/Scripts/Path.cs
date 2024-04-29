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
using UnityEngine.Rendering.Universal.Internal;
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
            Node newChild = new Node(this, move, silenceIn);
            if (grid != null) {
                NumberGrid gridCopy = new NumberGrid(grid);
                gridCopy.update(newChild.relativePosition);
                newChild.grid = gridCopy;
            }
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

    public Vector2Int startingPosition;
    private Node head;
    private Node[] tails;
    public int nodeCount;
    public Tilemap pathTilemap;
    public Tilemap elimTilemap;
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
    public Tile elimTile;
    private Vector2Int[] elims;

    public void startPath(Vector2Int move) {
        head = new Node(null, Vector2Int.zero);
        tails = new Node[1] {new Node(head, move)};
        nodeCount = 2;
    }

    public void startPath(Vector2Int move, Vector2Int startingPosition) {
        head = new Node(null, Vector2Int.zero);
        tails = new Node[1] {new Node(head, move)};
        nodeCount = 2;
        this.startingPosition = startingPosition;
    }

    public void startPath(Vector2Int move, NumberGrid grid) {
        head = new Node(null, Vector2Int.zero);
        tails = new Node[1] {new Node(head, move)};
        grid.update(move);
        tails[0].setGrid(grid);
        elims = new Vector2Int[] {};
        nodeCount = 2;
        reconstructElimDisplay();
    }

    public void startPath(Vector2Int move, Vector2Int startingPosition, NumberGrid grid) {
        head = new Node(null, Vector2Int.zero);
        tails = new Node[1] {new Node(head, move)};
        grid.update(move);
        tails[0].setGrid(grid);
        elims = new Vector2Int[] {};
        nodeCount = 2;
        this.startingPosition = startingPosition;
        reconstructElimDisplay();
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
        Node oldTail = tails[i];
        tails[i] = oldTail.addChild(move, silenceIn);
        oldTail.setGrid(null);
        nodeCount += 1;
        if (elimTilemap != null) {
            updateElimDisplay();
        }
        return tails[i];
    }

    public Node[] extendTail(int tailsIndex, Vector2Int[] moves) {
        Node oldTail = tails[tailsIndex];
        Node[] newTails = {};
        foreach (Vector2Int move in moves) {
            newTails.Concat(new Node[] {oldTail.addChild(move, true)});
            nodeCount += 1;
        }
        tails = tails.Where(var => var != oldTail).ToArray();
        oldTail.setGrid(null);
        if (elimTilemap != null) {
            updateElimDisplay();
        }
        return newTails;
    }

    public void clearPath() {
        head = null;
        tails = new Node[] {};
        nodeCount = 0;
        pathTilemap.ClearAllTiles();
    }
    
    public void setTails(Node[] tails) {
        this.tails = tails;
    }

    public Node[] getTails() {
        return tails != null ? tails : null;
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
        reconstructDisplay();
        if (elimTilemap != null) {
            reconstructElimDisplay();
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
        return isCollisionHelper(relativePosition, tail);
    }

    private bool isCollisionHelper(Vector2Int relativePosition, Node node) {
        if (node.getRelativePosition() == relativePosition) {
            return true;
        } else if (node.getParent() == null) {
            return false;
        }
        return isCollisionHelper(relativePosition, node.getParent());
    }

    public void updateDisplay(Vector2Int move, Vector2Int position) {
        pathTilemap.SetTile(pathTilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), start);
        pathTilemap.SetTransformMatrix(pathTilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(move)), Vector3.one));
        Vector3Int targetPosition = new Vector3Int(position.x + move.x, position.y + move.y, 0);
        pathTilemap.SetTile(pathTilemap.WorldToCell(targetPosition), endpoint);
        pathTilemap.SetTransformMatrix(pathTilemap.WorldToCell(targetPosition), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(move)), Vector3.one));
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
        pathTilemap.SetTile(pathTilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), tile);
        int shouldFlip = this.shouldFlip(lastMove, move);
        // pathTilemap.SetTransformMatrix(pathTilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlip, 90 * quarterTurns(lastMove, shouldFlip)), Vector3.one));
        pathTilemap.SetTransformMatrix(pathTilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlip, 90 * ((quarterTurns(lastMove) + 2 * shouldFlip * (lastMove.x % 2)) % 4)), Vector3.one));
        Vector3Int targetPosition = new Vector3Int(position.x + move.x, position.y + move.y, 0);
        Tile endpointTile = toSilence ? silenceEndpoint : endpoint;
        pathTilemap.SetTile(pathTilemap.WorldToCell(targetPosition), endpointTile);
        pathTilemap.SetTransformMatrix(pathTilemap.WorldToCell(targetPosition), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(move)), Vector3.one));
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
        pathTilemap.SetTile(pathTilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), tile);
        pathTilemap.SetTransformMatrix(pathTilemap.WorldToCell(new Vector3Int(position.x, position.y, 0)), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180 * shouldFlipBranch, 90 * ((quarterTurns(lastMove) + 2 * shouldFlipBranch * (lastMove.x % 2)) % 4)), Vector3.one));
        foreach (Vector2Int move in moves) {
            Vector3Int targetPosition = new Vector3Int(position.x + move.x, position.y + move.y, 0);
            pathTilemap.SetTile(pathTilemap.WorldToCell(targetPosition), silenceEndpoint);
            pathTilemap.SetTransformMatrix(pathTilemap.WorldToCell(targetPosition), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * quarterTurns(new Vector2Int(move.x, move.y))), Vector3.one));
        }
    }

    public void reconstructDisplay() {
        pathTilemap.ClearAllTiles();
        updateDisplay(head.getMove(), startingPosition);
        reconstructDisplayHelper(head);
    }

    private int reconstructDisplayHelper(Node parent) {
        Node[] children = parent.getChildren();
        if (children.Length == 1) {
            updateDisplay(parent.getMove(), children[0].getMove(), startingPosition + parent.getRelativePosition(), children[0].getSilenceIn(), children[0].getSilenceOut());
        } else if (children.Length > 1) {
            Vector2Int[] moves = {};
            foreach (Node child in children) {
                moves.Append(child.getMove());
            }
            updateDisplay(parent.getMove(), moves, startingPosition + parent.getRelativePosition());
        } else {
            return 0;
        }
        foreach (Node child in children) {
            reconstructDisplayHelper(child);
        }
        return 1;
    }

    public void updateElimDisplay() {
        int mapHeight = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>().mapHeight;
        int mapWidth = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogicManager>().mapWidth;
        for (int x = 1; x <= mapWidth; x++) {
            for (int y = 1; y <= mapHeight; y++) {
                Vector2Int newElim = new Vector2Int(x - 5, y - 5);
                bool update = true;
                foreach (Node tail in tails) {
                    if (tail.getGrid().getValue(x, y) != 1) {
                        update = false;
                    }
                }
                if (update && !elims.Contains(newElim)) {
                    Debug.Log("Setting ElimTile at (" + x + ", " + y + ")");
                    elimTilemap.SetTile(elimTilemap.WorldToCell(new Vector3(newElim.x, -newElim.y)), elimTile);
                    elims.Append(newElim);
                }
            };
        };
    }

    public void reconstructElimDisplay() {
        elimTilemap.ClearAllTiles();
        updateElimDisplay();
    }

    public int quarterTurns(Vector2Int move) {
        return ((move.y + 1) * (move.y % 2)) + ((move.x + 4) % 4);
    }

    public int shouldFlip(Vector2Int lastMove, Vector2Int move) {
        return (lastMove.x - move.y + 3) % 4 / 3 * ((lastMove.y + move.x + 3) % 4 / 3); // math based on matrix for clockwise rotation about the origin
    }

    public int shouldFlip(Vector2Int lastMove, Vector2Int[] moves) {
        return ((lastMove.x - moves[0].y + 3) % 4 / 3 * ((lastMove.y + moves[0].x + 3) % 4 / 3)) + ((lastMove.x - moves[1].y + 3) % 4 / 3 * ((lastMove.y + moves[1].x + 3) % 4 / 3)); // math based on matrix for clockwise rotation about the origin
    }
}