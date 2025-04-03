using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class PivotLocator : MonoBehaviour
{
    [SerializedDictionary, SerializeField] private SerializedDictionary<WorkType, List<Transform>> idleAreas;
    [SerializedDictionary, SerializeField] private SerializedDictionary<CookwareType,List<Transform>> cookWarePivots;
    [SerializeField] private List<Transform> tablePivots;
    [SerializeField] private List<Transform> pickupCounterPivots;
    [SerializeField] private List<Transform> watingLinePivots;
    [SerializeField] private Transform consumerSpawnPivot;
    [SerializeField] private Transform counterPivot;
    [SerializeField] private Transform trayReturnCounterPivot;
    [SerializeField] private Transform sinkPivot;
    
    
    public SerializedDictionary<WorkType, List<Transform>> IdleAreas => idleAreas;
    public SerializedDictionary<CookwareType, List<Transform>> CookWarePivots => cookWarePivots;
    public List<Transform> TablePivots => tablePivots;
    public List<Transform> PickupCounterPivots => pickupCounterPivots;
    public List<Transform> WatingLinePivots => watingLinePivots;
    public Transform ConsumerSpawnPivot => consumerSpawnPivot;
    public Transform CounterPivot => counterPivot;
    public Transform TrayReturnCounterPivot => trayReturnCounterPivot;
    public Transform SinkPivot => sinkPivot;
}
