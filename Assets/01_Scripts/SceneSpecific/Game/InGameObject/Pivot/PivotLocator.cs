using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class PivotLocator : MonoBehaviour
{
    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<WorkType, List<Transform>> idleAreas;

    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<CookwareType, List<Transform>> cookWarePivots;

    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<int, Transform> interiorPivots;

    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<ObjectArea, Transform> trashCanPivots;
    
    [SerializeField] private List<Transform> tablePivots;
    [SerializeField] private List<Transform> pickupCounterPivots;
    [SerializeField] private List<Transform> watingLinePivots;
    [SerializeField] private List<Transform> payWaitingPivots;
    [SerializeField] private Transform consumerSpawnPivot;
    [SerializeField] private Transform counterPivot;
    [SerializeField] private Transform trayReturnCounterPivot;
    [SerializeField] private Transform sinkPivot;

    [SerializeField] private Transform hallRight;
    [SerializeField] private Transform hallLeft;
    [SerializeField] private Transform kitchenRight;
    [SerializeField] private Transform kitchenLeft;
    
    public SerializedDictionary<WorkType, List<Transform>> IdleAreas => idleAreas;
    public SerializedDictionary<CookwareType, List<Transform>> CookWarePivots => cookWarePivots;
    public SerializedDictionary<int, Transform> InteriorPivots => interiorPivots;
    public SerializedDictionary<ObjectArea, Transform> TrashCanPivots => trashCanPivots;
    public List<Transform> TablePivots => tablePivots;
    public List<Transform> PickupCounterPivots => pickupCounterPivots;
    public List<Transform> WatingLinePivots => watingLinePivots;
    public List<Transform> PayWaitingPivots => payWaitingPivots;
    public Transform ConsumerSpawnPivot => consumerSpawnPivot;
    public Transform CounterPivot => counterPivot;
    public Transform TrayReturnCounterPivot => trayReturnCounterPivot;
    public Transform SinkPivot => sinkPivot;
    public Transform HallRight => hallRight;
    public Transform HallLeft => hallLeft;
    public Transform KitchenRight => kitchenRight;
    public Transform KitchenLeft => kitchenLeft;
}