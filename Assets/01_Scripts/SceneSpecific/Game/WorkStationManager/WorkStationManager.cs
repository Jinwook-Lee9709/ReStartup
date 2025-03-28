using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class WorkStationManager : MonoBehaviour
{
    [SerializeField] private WorkFlowController workFlowController;
    [SerializeField] private Button button;
    [SerializeField] private List<Transform> tablePivots;
    [SerializeField] private NavMeshSurface surface2D;

    public int currentTableCount = 0;

    private void Start()
    {
        button.onClick.AddListener(OnTableAdd);
        int excludeLayer = LayerMask.NameToLayer("InGameText");
        surface2D.layerMask = ~(1 << excludeLayer);
        surface2D.BuildNavMesh();
    }

    public void OnTableAdd()
    {
        if (currentTableCount < tablePivots.Count)
        {
            Debug.Log("Table Add");
            Addressables.InstantiateAsync("Table").Completed += OnTableInstantiated;
        }
    }

    private void OnTableInstantiated(AsyncOperationHandle<GameObject> obj)
    {
        currentTableCount++;
        var table = obj.Result.GetComponent<Table>();
        obj.Result.transform.SetParent(tablePivots[currentTableCount - 1]);
        obj.Result.transform.localPosition = Vector3.zero;
        obj.Result.transform.localRotation = Quaternion.identity;
        obj.Result.transform.localScale = Vector3.one;
        table.SetId(currentTableCount);
        workFlowController.AddTable(table);

        surface2D.UpdateNavMesh(surface2D.navMeshData);

        if (currentTableCount == tablePivots.Count)
        {
            button.interactable = false;
        }
    }
}