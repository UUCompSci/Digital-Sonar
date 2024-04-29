using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class NumberGrid {
    public int mapWidth;
    public int mapHeight;
    private int[,] gridInfo;
    private int numIslandTiles;
    private static Vector2Int[] islands = new Vector2Int[] { 
        new Vector2Int(3, 2),
        new Vector2Int(7, 2),
        new Vector2Int(5, 3),
        new Vector2Int(9, 3),
        new Vector2Int(2, 4),
        new Vector2Int(7, 5),
        new Vector2Int(2, 6),
        new Vector2Int(5, 6),
        new Vector2Int(9, 6),
        new Vector2Int(6, 7),
        new Vector2Int(3, 8),
        new Vector2Int(8, 9),
    };
    private GameLogicManager gameLogicManager = GameObject.Find("Game Logic Manager").GetComponent<GameLogicManager>();

    private int maxMeaningfulValue = 4; // The highest value a tile on the grid can meaningfully have

    public NumberGrid() {
        mapWidth = gameLogicManager.mapWidth;
        mapHeight = gameLogicManager.mapHeight;
        // the extra 2 columns and rows enable the application of regular collision logic from islands for map borders as well

        gridInfo = new int[mapWidth, mapHeight];

        validateIslands();

        foreach (Vector2Int island in islands) {
            gridInfo[island.x - 1, island.y - 1] = 2;
        };
    }

    public NumberGrid(NumberGrid grid) {
        this.mapWidth = grid.mapWidth;
        this.mapHeight = grid.mapHeight;
        this.gridInfo = grid.gridInfo;
    }

    public static void setIslands(Vector2Int[] islands) {
        NumberGrid.islands = islands;
    }

    private bool validateIslands() {
        foreach (Vector2Int island in islands) {
            if (!(validateColumn(island.x) && validateRow (island.y))) {
                return false;
            };
        }
        return true;
    }

    public bool validateColumn(int column) {
        if (column < 1 || column > mapHeight) {
            return false;
        };
        return true;
    }
    
    public bool validateRow (int row) {
        if (row < 1 || row > mapWidth) {
            return false;
        };
        return true;
    }

    public bool validateValue(int value) {
        if (value < 0 || value > maxMeaningfulValue) {
            return false;
        };
        return true;
    }

    public int getValue(int column, int row) {
        if (validateColumn(column) && validateRow (row)) {
            return gridInfo[column - 1, row - 1];
        } else {
            if (!validateColumn(column)) {
                Debug.LogError($"Invalid column number {column} inputted to Grid.getValue");
            };
            if (!validateRow (row)) {
                Debug.LogError($"Invalid row number {row} inputted to Grid.getValue");
            };
            return -1;
        };
    }

    public void setValue(int column, int row, int value) {
        if (validateColumn(column) && validateRow (row) && validateValue(value)) {
            gridInfo[column - 1, row - 1] = value;
        } else {
            if (!validateColumn(column)) {
                Debug.LogError($"Invalid column number {column} inputted to Grid.setValue");
            };
            if (!validateRow (row)) {
                Debug.LogError($"Invalid row number {row} inputted to Grid.setValue");
            };
            if (!validateValue(value)) {
                Debug.LogError($"Invalid value {value} inputted to Grid.setValue");
            };
        };
    }

    public void eliminateColumn(int column) {
        if (validateColumn(column)) {
            for (int i = 0; i < mapHeight; i++) {
                if (gridInfo[column - 1, i] != 2) {
                    gridInfo[column - 1, i] = 1;
                }
            }
        }
    }
    
    public void eliminateRow (int row) {
        if (validateRow (row)) {
            for (int i = 0; i < mapHeight; i++) {
                if (gridInfo[i, row - 1] != 2) {
                    gridInfo[i, row - 1] = 1;
                }
            }
        }
    }
        
    public void update(Vector2Int relativePosition) {
        foreach (Vector2Int island in islands) {
            int column = island.x - relativePosition.x;
            int row = island.y + relativePosition.y;
            if (validateColumn(column) && validateRow (row) && gridInfo[column - 1, row - 1] != 2) {
                gridInfo[column - 1, row - 1] = 1;
            }
        }
    }
    
    public bool isFull() {
        for (int x = 0; x < mapHeight; x++) {
            for (int y = 0; y < mapWidth; y++) {
                if (gridInfo[x, y] == 0) {
                    return false;
                };
            };
        };
        return true;
    }

}