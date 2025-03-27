using NavMeshPlus.Components;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] public NavMeshSurface surface;

    private void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
        ServiceLocator.Instance.GetSceneService<GameManager>();
    }

    [ContextMenu("Test")]
    public void Test2()
    {
        surface.UpdateNavMesh(surface.navMeshData);
    }
}