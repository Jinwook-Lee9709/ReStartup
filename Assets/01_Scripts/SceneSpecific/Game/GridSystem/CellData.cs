using UnityEngine;

public class CellData : MonoBehaviour
{
    public Vector2Int Position { get; private set; }
    public GameObject ObjectOnCell { get; private set; }

    public bool IsOccupied => ObjectOnCell != null;
}