using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Grid {
    private int grid_width;
    private int map_width;
    private int grid_height;
    private int map_height;
    private int[,] gridInfo;
    private static int numIslandTiles;
    private int[,] islands = new int[2, 2] { 
        {2, 5},
        {3, 4} 
    };

    private int maxMeaningfulValue = 4; // The highest value a tile on the grid can meaningfully have

    public Grid(int width, int height) {
        this.grid_width = width + 2;
        this.map_width = width;
        this.grid_height = height + 2;
        this.map_height = height;
        // the extra 2 rows and columns enable the application of regular collision logic from islands for map borders as well

        gridInfo = new int[grid_width, grid_height];  
        for (int row = 0; row < grid_height; row++) {
            gridInfo[row, 0] = 2;
            gridInfo[row, grid_width - 1] = 2;
        };

        for (int col = 1; col <= map_width; col++) { // smaller loop because the corners have already been set to 1
            gridInfo[0, col] = 2;
            gridInfo[grid_height - 1, col] = 2;
        };

        validateIslands(islands);

        for (int i = 0; i < numIslandTiles; i++) {
            gridInfo[islands[i, 0] + 1, islands[i, 1] + 1] = 2;
        };
    }

    private bool validateIslands(int[,] islands) {
        for (int i = 0; i < numIslandTiles; i++) {
            if (!(validateRow(islands[i, 0]) && validateColumn(islands[i, 1]))) {
                return false;
            };
        }
        return true;
    }

    public bool validateRow(int row) {
        if (row < 0 || row >= map_height) {
            return false;
        };
        return true;
    }
    
    public bool validateColumn(int col) {
        if (col < 0 || col >= map_width) {
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
        
    public bool isFull() {
        for (int x = 1; x <= map_height; x++) {
            for (int y = 1; y <= map_width; y++) {
                if (gridInfo[x,y] == 0) {
                    return false;
                };
            };
        };
        return true;
    }

}