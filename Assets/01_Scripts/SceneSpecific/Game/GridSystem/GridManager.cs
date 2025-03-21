using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Dictionary<Vector2Int, int> gridInfo;


    public bool IsOccupied(Vector2Int position)
    {
        return gridInfo.ContainsKey(position);
    }
    
}
