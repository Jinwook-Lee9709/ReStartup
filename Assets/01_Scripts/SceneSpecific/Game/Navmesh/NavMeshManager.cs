using NavMeshPlus.Components;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface surface;

    public void UpdateNavMesh()
    {
        surface.UpdateNavMesh(surface.navMeshData);
    }
}