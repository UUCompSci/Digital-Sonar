using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ViewSwapper : MonoBehaviour
{
    public SpriteRenderer submarine;
    public TilemapRenderer radioOperatorPath;
    public TilemapRenderer radioOperatorStartsGrid;
    public TilemapRenderer captainGridPath;
    public Canvas moveTargets;

    public void swapView() {
        radioOperatorStartsGrid.enabled = submarine.enabled;
        submarine.enabled = !radioOperatorStartsGrid.enabled;
        captainGridPath.enabled = submarine.enabled;
        moveTargets.enabled = submarine.enabled;
    }
}
