using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class NumberGrid {
    private int gridWidth;
    private int mapWidth;
    private int gridHeight;
    private int mapHeight;
    private int[,] gridInfo;
    private static int numIslandTiles;
    private static Vector2Int[] islands = new Vector2Int[2] { 
        new Vector2Int(2, 5),
        new Vector2Int(3, 4) 
    };
    //get island tilemap

    private int maxMeaningfulValue = 4; // The highest value a tile on the grid can meaningfully have

    public NumberGrid(int width, int height) {
        this.gridWidth = width + 2;
        this.mapWidth = width;
        this.gridHeight = height + 2;
        this.mapHeight = height;
        // the extra 2 rows and columns enable the application of regular collision logic from islands for map borders as well

        gridInfo = new int[gridWidth, gridHeight];  
        for (int row = 0; row < gridHeight; row++) {
            gridInfo[row, 0] = 2;
            gridInfo[row, gridWidth - 1] = 2;
        };

        for (int col = 1; col <= mapWidth; col++) { // smaller loop because the corners have already been set to 1
            gridInfo[0, col] = 2;
            gridInfo[gridHeight - 1, col] = 2;
        };

        validateIslands(islands);

        for (int i = 0; i < numIslandTiles; i++) {
            gridInfo[islands[i].x + 1, islands[i].y + 1] = 2;
        };
    }

    public NumberGrid(NumberGrid grid) {
        this.gridWidth = grid.gridWidth;
        this.gridHeight = grid.gridHeight;
        this.mapWidth = grid.mapWidth;
        this.mapHeight = grid.mapHeight;
        this.gridInfo = grid.gridInfo;
    }

    private bool validateIslands(Vector2Int[] islands) {
        for (int i = 0; i < numIslandTiles; i++) {
            if (!(validateRow(islands[i].x) && validateColumn(islands[i].y))) {
                return false;
            };
        }
        return true;
    }

    public bool validateRow(int row) {
        if (row < 0 || row >= mapHeight) {
            return false;
        };
        return true;
    }
    
    public bool validateColumn(int col) {
        if (col < 0 || col >= mapWidth) {
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

    public int getValue(int row, int col) {
        if (validateRow(row) && validateColumn(col)) {
            return gridInfo[row + 1, col + 1];
        } else {
            if (!validateRow(row)) {
                Console.WriteLine($"Invalid row number {row} inputted to Grid.getValue");
            };
            if (!validateColumn(col)) {
                Console.WriteLine($"Invalid column number {col} inputted to Grid.getValue");
            };
            return -1;
        };
    }

    public void setValue(int row, int col, int value) {
        if (validateRow(row) && validateColumn(col) && validateValue(value)) {
            gridInfo[row + 1, col + 1] = value;
        } else {
            if (!validateRow(row)) {
                Console.WriteLine($"Invalid row number {row} inputted to Grid.setValue");
            };
            if (!validateColumn(col)) {
                Console.WriteLine($"Invalid column number {col} inputted to Grid.setValue");
            };
            if (!validateValue(value)) {
                Console.WriteLine($"Invalid value {value} inputted to Grid.setValue");
            };
        };
    }
        
    public void update(Vector2Int relativePosition) {
        for (int i = 0; i < islands.Length; i++) {
            if (gridInfo[islands[i].x - relativePosition.x + 1, islands[i].y - relativePosition.y + 1] == 2) {
                gridInfo[islands[i].x - relativePosition.x + 1, islands[i].y - relativePosition.y + 1] = 1;
            }
        }
    }
    
    public bool isFull() {
        for (int x = 1; x <= mapHeight; x++) {
            for (int y = 1; y <= mapWidth; y++) {
                if (gridInfo[x,y] == 0) {
                    return false;
                };
            };
        };
        return true;
    }

}