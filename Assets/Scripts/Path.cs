using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class Path {
    public class Node {
        private Node parent;
        private Node[] children;
        private int[] move;
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
    }

    private Node head;
    private Node[] tails;

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
}