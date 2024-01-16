using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
    private int grid_width;
    private int map_width;
    private int grid_height;
    private int map_height;
    private int[,] gridInfo;
    private int[, 2] islands;

    public Grid(int width; int height) {
        this.grid_width = width + 2;
        this.map_width = width;
        this.grid_height = height + 2;
        this.map_height = height;
        // the extra 2 rows and columns enable the application of regular collision logic from islands for map borders as well

        gridInfo = new int[grid_width, grid_height];  
        for (new int row = 0; row < grid_height; row++) {
            gridInfo[row][0] = 1;
            gridInfo[row][grid_width - 1] = 1;
        };

        for (new int col = 1; col < grid_width - 1; col++) {
            gridInfo[0][col] = 1;
            gridInfo[grid_height - 1][col] = 1;
        };

        validateIslands(islands);

        for (new int i = 0; i < sizeof islands; i++) {
            gridInfo[islands[i][0] + 1][islands[i][1] + 1] = 1;
        };
    };

    private bool validateIslands(int[, 2] islands) {
        for (new int i = 0; i < sizeof islands; i++) {
            if (!(0 <= islands[i][0] < map_height) or !(0 <= islands[i][1] < map_width)) {
                return false;
            };
        };
        return true;
    };

    private bool validateIndex(int row, int col) {
        if (!(0 <= row < map_height) or !(0 <= col < map_width)) {
            return false;
        };
        return true;
    }

    public int getValue(int row, int col) {
        if (row input invalid) {
            throw new RowOutOfBoundsException();
        } else if (column input invalid) {
            throw new ColumnOutOfBoundsException();
        } else {
            return gridInfo[row + 1, col + 1];
        };
    };

    public void setValue(int row, int col, int value) {
        if (row input invalid) {
            throw new RowOutOfBoundsException();
        } else if (column input invalid) {
            throw new ColumnOutOfBoundsException();
        } else if (value input invalid) {
            throw new InvalidValueException();
        } else {
            gridInfo[row + 1, col + 1] = value;
        };
    };
        
    public bool isFull() {
        // if I can find a library to use min() then return true if min(gridInfo) > 0
        // if not, return false if there are any 0s in gridInfo, else return true
    };

}
